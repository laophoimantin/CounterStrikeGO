using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class EnemyController : PawnUnit, INoiseListener, IFlashable, IBurnable
{
    [Header("References")]
    private EnemyVisual _enemyVisual;
    public EnemyVisual EnemyVisual => _enemyVisual;
    [SerializeField] private UnitCombat _unitCombat;
    [SerializeField] private EnemyMovement _enemyMovement;

    private BaseEnemyBehavior _currentBehavior;
    [SerializeField] private BaseEnemyBehavior _defaultBehavior;
    [SerializeField] private FollowingNoiseBehavior _noiseBehavior;
    [SerializeField] private FlashedBehavior _flashedBehavior;

    [Header("Enemy State")]
    [SerializeField] private Direction _facingDirection = Direction.None;
    private State _currentState;
    public bool IsFlashed => _currentState == State.Flashed;

    private readonly Direction[] _dirs =
    {
        Direction.North,
        Direction.East,
        Direction.South,
        Direction.West
    };

    public Direction CurrentFacingDirection => _facingDirection;
    private List<Node> _astarPath = new();

    public Node StartNode => 0 < _astarPath.Count ? _astarPath[0] : null;
    public Node NextNode => 1 < _astarPath.Count ? _astarPath[1] : null;
    public Node UpcomingNode => 2 < _astarPath.Count ? _astarPath[2] : null;


    public Action<EnemyController> OnDeath;

    private int _flashTurnsRemaining;


    void OnEnable()
    {
        EnemyManager.Instance.RegisterEnemy(this);
    }

    void Awake()
    {
        if (_currentNode == null)
        {
            Debug.LogWarning($"{gameObject.name} has no node assigned!!!");
            return;
        }

        if (_defaultBehavior == null || _noiseBehavior == null)
        {
            Debug.LogWarning($"{gameObject.name} has no behavior assigned!!!");
            return;
        }

        if (_facingDirection == Direction.None)
        {
            Debug.LogWarning($"{gameObject.name} has no facing direction assigned!!!");
            return;
        }

        _enemyVisual = _visual as EnemyVisual;
    }

    void Start()
    {
        if (_currentNode != null)
            _currentNode.AddUnit(this);
        ChangeState(State.Normal);
    }

    private void ChangeState(State newState)
    {
        if (_currentState == newState)
            return;

        ExitState(_currentState);
        _currentState = newState;
        EnterState(newState);
    }

    private void EnterState(State state)
    {
        switch (state)
        {
            case State.Normal:
                _currentBehavior = _defaultBehavior;
                break;

            case State.Flashed:
                _currentBehavior = _flashedBehavior;
                break;

            case State.Distracted:
                _currentBehavior = _noiseBehavior;
                break;
        }
    }

    private void ExitState(State state)
    {
        switch (state)
        {
            case State.Normal:
                break;

            case State.Flashed:
                _enemyVisual.HideStunIcon();
                break;

            case State.Distracted:
                _enemyVisual.HideQuestionIcon();
                break;
        }
    }


    public void StartAction()
    {
        StartCoroutine(ExecuteBehavior());
    }

    private IEnumerator ExecuteBehavior()
    {
        if (_isDead)
        {
            FinishTurn();
            yield break;
        }

        List<BaseEnemyAction> plan = _currentBehavior.PlanActions(this);

        if (plan == null || plan.Count == 0)
        {
            FinishTurn();
            yield break;
        }

        foreach (BaseEnemyAction action in plan)
        {
            yield return action.Execute(this);

            Tween attackTween = _unitCombat.GetAttackTween(_currentNode);

            if (attackTween != null)
            {
                yield return attackTween.WaitForCompletion();
                break;
            }
        }

        FinishTurn();
    }

    private void FinishTurn()
    {
        AdvancePath();
        AdvanceFlashed();
        EvaluateState();
        EnemyManager.Instance.OnEnemyFinished(this);
    }

    private void EvaluateState()
    {
        if (!HasEndFlashed())
        {
            ChangeState(State.Flashed);
        }
        else if (!HasReachedNoiseDestination())
        {
            ChangeState(State.Distracted);
        }
        else
        {
            ChangeState(State.Normal);
        }
    }

    // --- AI Action & Utility Methods ---
    // Actions ==============================================================================================

    #region Actions Methods

    public bool ScanForTargetInFront(int range)
    {
        Node nodeToScan = GetNodeInFront();

        while (nodeToScan != null && range > 0)
        {
            if (!nodeToScan.IsWalkable()) return false;

            if (nodeToScan.IsHideable()) return false;

            foreach (GridOccupant occupant in nodeToScan.GetAllOccupants())
            {
                if (occupant is PawnUnit victim && IsEnemyOf(victim))
                {
                    return true;
                }
            }

            range--;
            nodeToScan = nodeToScan.GetNodeInDirection(_facingDirection);
        }

        return false;
    }


    // Basic Movement
    public Sequence Move(Node targetNode)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { ChangeNode(targetNode); });
        seq.Append(_visual.MoveTo(targetNode.WorldPos, _actionDuration));

        return seq;
    }

    public Sequence Rotate(Direction newDirection)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { SetFacingDirection(newDirection); });
        Quaternion targetRot = GridMathUtility.GetRotation(newDirection);
        seq.Append(_visual.RotateTo(targetRot, _actionDuration));
        return seq;
    }

    // Die
    public override Tween Die()
    {
        if (_isDead) return null;
        _isDead = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(_enemyVisual.Wobble());
        seq.Append(_visual.FlyUp());
        seq.AppendCallback(() =>
        {
            UnAssignCurrentNode();
            OnDeath?.Invoke(this);
        });
        seq.OnComplete(() => { FinishDeath(); });
        return seq;
    }

    private void FinishDeath()
    {
        Sequence seq = DOTween.Sequence();

        Vector3 finalRestingPlace = EnemyGraveyardManager.Instance.GetNextSlotPosition();

        seq.AppendCallback(() => { SnapPosition(finalRestingPlace); });

        seq.Append(_enemyVisual.DropDown());
        seq.Append(_enemyVisual.Bounce());
    }


    // Molotov-ed
    public Tween Burn()
    {
        Sequence seq = DOTween.Sequence();

        Node targetNode = FindWalkableNodeFromFacing();

        // Cannot find a walkable node, so die
        if (targetNode == null)
        {
            seq.Append(Die());
            return seq;
        }

        // Runaway
        Direction targetDir = GridMathUtility.GetDirectionFromTargetNode(_currentNode, targetNode);

        if (targetDir != _facingDirection)
            seq.Append(Rotate(targetDir));

        seq.Append(Move(targetNode));
        return seq;
    }

    private Node FindWalkableNodeFromFacing()
    {
        for (int i = 0; i < 4; i++)
        {
            Direction dir = GridMathUtility.GetDirectionByStep(_facingDirection, i);
            Node node = _currentNode.GetNodeInDirection(dir);
            if (node != null && node.IsWalkable())
                return node;
        }

        return null;
    }

    // Decoy-ed
    public Tween HearNoise(Node noiseOrigin)
    {
        if (IsFlashed || _isDead)
            return null;

        _astarPath = AstarPathfinder.FindPath(_currentNode, noiseOrigin);

        if (_astarPath == null || _astarPath.Count < 2)
            return null;

        Sequence seq = DOTween.Sequence();
        Direction targetDirection = GridMathUtility.GetDirectionFromTargetNode(_currentNode, NextNode);

        seq.Append(_enemyVisual.ShowQuestionIcon());
        seq.Append(Rotate(targetDirection));

        seq.OnComplete(() => { ChangeState(State.Distracted); });
        return seq;
    }

    private void AdvancePath()
    {
        if (_astarPath != null && _astarPath.Count > 0)
            _astarPath.RemoveAt(0);
    }

    public bool HasReachedNoiseDestination()
    {
        return _astarPath == null || _astarPath.Count <= 1;
    }

    // Flash-ed
    public Tween GetFlashed(int duration)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
            {
                ChangeState(State.Flashed);
                _flashTurnsRemaining = Mathf.Max(_flashTurnsRemaining, duration);
                _astarPath?.Clear();
            }
        );
        seq.Append(_enemyVisual.Wobble());
        seq.JoinCallback(() => { _enemyVisual.ShowStunIcon(); }
        );
        return seq;
    }

    private void AdvanceFlashed()
    {
        if (_flashTurnsRemaining > 0)
            _flashTurnsRemaining--;
    }

    public bool HasEndFlashed()
    {
        return _flashTurnsRemaining <= 0;
    }

    #endregion

    private void SnapPosition(Vector3 targetPos)
    {
        transform.position = new Vector3(targetPos.x, targetPos.y, targetPos.z);
        _visual.SetPosition(transform.position);
    }

   
    public void SetFacingDirection(Direction newDirection)
    {
        _facingDirection = newDirection;
    }

    public Node GetNodeInFront()
    {
        return _currentNode.GetNodeInDirection(_facingDirection);
    }

    public Node GetNodeInBack()
    {
        return _currentNode.GetNodeInDirection(GridMathUtility.TurnAround(_facingDirection));
    }

    // Editor ====================================================================================

    #region Editor Methods

#if UNITY_EDITOR
    public void SetDirection(Direction dir)
    {
        _visual.SetRotation(GridMathUtility.GetRotation(dir));
        SetFacingDirection(dir);
    }
    public void SetOrMoveNode(Direction? dir = null)
    {
        NodeManager manager = NodeManager.Instance;
        if (manager == null)
            manager = FindObjectOfType<NodeManager>();

        if (manager == null)
        {
            Debug.LogError("No NodeManager found in scene. Cannot move.", this);
            return;
        }

        Node newNode;
        string warningLog = dir.HasValue ? " No valid node to move to!" : " Invalid spot, couldn't find a node!";

        if (dir.HasValue)
        {
            newNode = _currentNode.GetNodeInDirection(dir.Value);
        }
        else
        {
            newNode = manager.GetNodeFromWorldPosition(transform.position);
        }

        if (newNode == null)
        {
            Debug.LogWarning(name + " has no valid node!" + warningLog, this);
            return;
        }

        if (_currentNode != null)
        {
            _currentNode.RemoveUnit(this);
            EditorUtility.SetDirty(_currentNode);
        }

        ChangeNode(newNode);
        transform.position = newNode.WorldPos;
        _visual.SetPosition(transform.position);

        EditorUtility.SetDirty(this);
    }
#endif

    #endregion
}

public enum State
{
    Normal,
    Flashed,
    Distracted
}