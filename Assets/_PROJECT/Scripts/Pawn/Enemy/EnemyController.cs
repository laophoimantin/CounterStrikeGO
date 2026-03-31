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
    private UnitCombat _unitCombat;

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
    public Node NextNode => 0 + 1 < _astarPath.Count ? _astarPath[1] : null;
    public Node UpcomingNode => 0 + 2 < _astarPath.Count ? _astarPath[2] : null;


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
        _currentBehavior = _defaultBehavior;
        
        _unitCombat = GetComponent<UnitCombat>();
    }
    void Start()
    {
        if (_currentNode != null)
            _currentNode.AddUnit(this);
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
        
        // foreach (BaseEnemyAction action in plan)
        // {
        //     yield return action.Execute(this);
        //
        //     if (TryAttack(_currentNode))
        //     {
        //         yield break;
        //     }
        // }

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

    public bool ScanForPlayerInFront(int range)
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
            nodeToScan = GetNodeInDirection(nodeToScan, _facingDirection);
        }

        return false; 
        
        // Node nodeToScan = GetNodeInFront();
        //
        // while (nodeToScan != null && range > 0)
        // {
        //     if (!nodeToScan.IsWalkable()) return false; 
        //     if (nodeToScan.IsHideable()) return false; 
        //
        //     if (nodeToScan.HasUnitsOfType<PlayerController>()) return true;
        //
        //     range--;
        //     nodeToScan = GetNodeInDirection(nodeToScan, _facingDirection);
        // }
        //
        // return false;
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
        Quaternion targetRot = GetRotationForDirection(newDirection);
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
        seq.OnComplete(() =>
        {
            FinishDeath();
        });
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
        Direction targetDir = GetDirectionFromTargetNode(_currentNode, targetNode);

        if (targetDir != _facingDirection)
            seq.Append(Rotate(targetDir));

        seq.Append(Move(targetNode));
        return seq;
    }

    private Node FindWalkableNodeFromFacing()
    {
        for (int i = 0; i < 4; i++)
        {
            Direction dir = GetDirectionByStep(i);
            Node node = GetNodeInDirection(_currentNode, dir);
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
        Direction targetDirection = GetDirectionFromCurrentNode(NextNode);

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
    
    #region Helper Methods

    // 1. STATE MANAGEMENT (Setters & Core Getters)
    // ==============================================================================================

    /// <summary>
    /// STATE CHANGE: Updates the internal facing direction state
    /// Call this only after a rotation action is complete
    /// </summary>
    public void SetFacingDirection(Direction newDirection)
    {
        _facingDirection = newDirection;
    }

    // 2. NODE QUERIES (Finding Neighbors)
    // ==============================================================================================

    /// <summary>
    /// Returns the node directly in front of the enemy based on current facing direction
    /// </summary>
    public Node GetNodeInFront()
    {
        return GetNodeInDirection(_currentNode, _facingDirection);
    }

    public Node GetNodeInBack()
    {
        return GetNodeInDirection(_currentNode, GetDirectionTurnAround());
    }

    public Node GetNodeInBackByStep(int step = 1)
    {
        Node targetNode = _currentNode;
        Direction backDirection = GetDirectionTurnAround();

        for (int i = step; i > 0; i--)
        {
            targetNode = GetNodeInDirection(targetNode, backDirection);
            if (targetNode == null) return null;
        }

        if (targetNode == _currentNode)
            return null;

        return targetNode;
    }


    /// <summary>
    /// Returns the neighbor of a specific node in a specific direction
    /// </summary>
    public Node GetNodeInDirection(Node node, Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return node.NorthNode;
            case Direction.South: return node.SouthNode;
            case Direction.East: return node.EastNode;
            case Direction.West: return node.WestNode;
            default: return null;
        }
    }

    // 3. DIRECTION LOGIC (Calculating Directions)
    // ==============================================================================================

    /// <summary>
    /// Calculates the direction from the current node to a target node
    /// </summary>
    public Direction GetDirectionFromCurrentNode(Node targetNode)
    {
        if (targetNode == null) return _facingDirection;
        return GetDirectionFromTargetNode(_currentNode, targetNode);
    }

    /// <summary>
    /// Calculates the direction between two adjacent nodes
    /// </summary>
    public Direction GetDirectionFromTargetNode(Node from, Node to)
    {
        if (to == null) return _facingDirection;

        if (from.NorthNode == to) return Direction.North;
        if (from.SouthNode == to) return Direction.South;
        if (from.EastNode == to) return Direction.East;
        if (from.WestNode == to) return Direction.West;

        return _facingDirection; // Default if not adjacent
    }

    /// <summary>
    /// Returns the Direction enum for a step (+1 for Clockwise)
    /// Useful for passing data to RotateActions
    /// </summary>
    private Direction GetDirectionByStep(int step)
    {
        int index = Array.IndexOf(_dirs, _facingDirection);
        int newIndex = (index + step + _dirs.Length) % _dirs.Length;
        return _dirs[newIndex];
    }

    // 4. ROTATION LOGIC (Math & Quaternions)
    // ==============================================================================================

    /// <summary>
    /// Calculates the rotation needed to face a specific direction
    /// </summary>
    private Quaternion GetRotationForDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Quaternion.Euler(0, 0, 0);
            case Direction.South: return Quaternion.Euler(0, 180, 0);
            case Direction.East: return Quaternion.Euler(0, 90, 0);
            case Direction.West: return Quaternion.Euler(0, -90, 0);
            default: return _visual.GetRotation();
        }
    }

    /// <summary>
    /// Calculates the rotation needed to face an adjacent target node
    /// </summary>
    public Quaternion GetRotationTowardsNode(Node targetNode)
    {
        Direction dirToNode = GetDirectionFromCurrentNode(targetNode);
        return GetRotationForDirection(dirToNode);
    }

    /// <summary>
    /// Helper to get a rotation based on steps (90 degree increments)
    /// </summary>
    private Quaternion GetRotationByStep(int step)
    {
        Direction futureDir = GetDirectionByStep(step);
        return GetRotationForDirection(futureDir);
    }


    // Rotation Presets ==============================================================================================

    public Quaternion GetRotationTurnAround() => GetRotationByStep(+2);
    public Quaternion GetRotationClockwise() => GetRotationByStep(+1);
    public Quaternion GetRotationCounterClockwise() => GetRotationByStep(-1);

    // Direction Presets ==============================================================================================

    public Direction GetDirectionTurnAround() => GetDirectionByStep(+2);
    public Direction GetDirectionClockwise() => GetDirectionByStep(+1);
    public Direction GetDirectionCounterClockwise() => GetDirectionByStep(-1);

    #endregion


    // Editor ====================================================================================

    #region Editor Methods

    public void SetDirection(Direction dir)
    {
        _visual.SetRotation(GetRotationForDirection(dir));
        SetFacingDirection(dir);
    }
#if UNITY_EDITOR
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
            newNode = GetNodeInDirection(_currentNode, dir.Value);
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
#endif
    }



 
    #endregion
}

public enum State
{
    Normal,
    Flashed,
    Distracted
}