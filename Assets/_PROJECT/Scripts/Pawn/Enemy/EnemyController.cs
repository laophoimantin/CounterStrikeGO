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
    [SerializeField] private UnitCombat _unitCombat;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private PathNavigator _pathNavigator;
    [SerializeField] private GridSensor _gridSensor;

    public EnemyVisual EnemyVisual => _enemyVisual;
    public UnitCombat UnitCombat => _unitCombat;
    public EnemyMovement EnemyMovement => _enemyMovement;
    public PathNavigator PathNavigator => _pathNavigator;
    public GridSensor GridSensor => _gridSensor;

    [Header("Facing Direction")]
    [SerializeField] private Direction _facingDirection = Direction.None;
    public Direction CurrentFacingDirection => _facingDirection;


    [Header("Enemy State")]
    private IEnemyState _currentState;

    [Header("State Instances")]
    public readonly NormalState StateNormal = new NormalState();
    public readonly FlashedState StateFlashed = new FlashedState();
    public readonly DistractedState StateDistracted = new DistractedState();

    [Header("Behavior")]
    private BaseEnemyBehavior _currentBehavior;
    [SerializeField] private BaseEnemyBehavior _defaultBehavior;
    [SerializeField] private FollowingNoiseBehavior _noiseBehavior;
    [SerializeField] private FlashedBehavior _flashedBehavior;

    public BaseEnemyBehavior DefaultBehavior => _defaultBehavior;
    public FollowingNoiseBehavior FollowingNoiseBehavior => _noiseBehavior;
    public FlashedBehavior FlashedBehavior => _flashedBehavior;
    private bool IsFlashed => _currentState == StateFlashed;

    public Action<EnemyController> OnDeath;


    void OnEnable()
    {
        EnemyManager.Instance.RegisterEnemy(this);
    }


    void Awake()
    {
        _enemyVisual = _visual as EnemyVisual;
    }

    void Start()
    {
        if (_currentNode != null)
        {
            SnapToNode(_currentNode);
        }
        
        ChangeState(StateNormal);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (_currentState == newState) return;

        _currentState?.ExitState(this);
        _currentState = newState;
        _currentState?.EnterState(this);
    }

    public void SetBehavior(BaseEnemyBehavior newBehav)
    {
        _currentBehavior = newBehav;
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
        _pathNavigator.AdvancePath();
        _currentState.ExecuteTurn(this);
        EnemyManager.Instance.OnEnemyFinished(this);
    }

    // Die
    public override Tween Die()
    {
        if (_isDead) return null;
        _isDead = true;

        Tween dieTween = _enemyVisual.GetDeathAnimation();
        
        if (dieTween != null)
        {
            dieTween.OnComplete(() =>
            {
                UnAssignCurrentNode();
                OnDeath?.Invoke(this);
            });
        }
        else
        {
            UnAssignCurrentNode();
            OnDeath?.Invoke(this);
        }
        return dieTween;
    }


    // Molotov-ed
    public Tween Burn()
    {
        Node targetNode = _gridSensor.FindEscapeNode(_facingDirection);

        // Cannot find any walkable node, so die
        if (targetNode == null)
        {
            return Die();
        }

        // Runaway
        Direction targetDir = GridMathUtility.GetDirectionFromTargetNode(_currentNode, targetNode);
        return _enemyMovement.GetBurnEscapeSeq(targetNode, targetDir, _actionDuration);
    }

    // Decoy-ed
    public Tween HearNoise(Node noiseOrigin)
    {
        if (IsFlashed || _isDead)
            return null;

        _pathNavigator.SetDestination(_currentNode, noiseOrigin);
        if (_pathNavigator.HasReachedDestination) return null;

        Direction targetDirection = GridMathUtility.GetDirectionFromTargetNode(_currentNode, _pathNavigator.NextNode);

        Sequence seq = DOTween.Sequence();
        seq.Append(_enemyVisual.ShowQuestionIcon());
        seq.Append(_enemyMovement.Rotate(targetDirection, _actionDuration));

        seq.OnComplete(() => { ChangeState(StateDistracted); });
        return seq;
    }

    // Flash-ed
    public Tween GetFlashed(int duration)
    {
        if (_isDead)
            return null;

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
            {
                StateFlashed.AddFlashDuration(duration);
                ChangeState(StateFlashed);
            }
        );

        seq.Append(_enemyVisual.Wobble());
        return seq;
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