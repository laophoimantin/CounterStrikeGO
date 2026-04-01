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
    public EnemyMovement EnemyMovement => _enemyMovement;
    public PathNavigator PathNavigator => _pathNavigator;
    public GridSensor GridSensor => _gridSensor;

    [Header("Behavior")]
    private BaseEnemyBehavior _currentBehavior;
    [SerializeField] private BaseEnemyBehavior _defaultBehavior;
    [SerializeField] private FollowingNoiseBehavior _noiseBehavior;
    [SerializeField] private FlashedBehavior _flashedBehavior;

    public BaseEnemyBehavior DefaultBehavior => _defaultBehavior;
    public FollowingNoiseBehavior FollowingNoiseBehavior => _noiseBehavior;
    public FlashedBehavior FlashedBehavior => _flashedBehavior;


    [Header("Enemy State")]
    [SerializeField] private Direction _facingDirection = Direction.None;
	private IEnemyState _currentState;

	private State _currentStateOld = State.Normal;

    public Direction CurrentFacingDirection => _facingDirection;

    private bool IsFlashed => _currentStateOld == State.Flashed;

    private int _flashTurnsRemaining;
    public Action<EnemyController> OnDeath;


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
        _currentBehavior = _defaultBehavior;
    }

	public void ChangeState(IEnemyState newState)
	{
		if (_currentState != null && _currentState.GetType() == newState.GetType())
		{
			return;
		}

		_currentState?.ExitState(this);
		_currentState = newState;
		_currentState?.EnterState(this);
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
        AdvanceFlashed();
        EvaluateState();
        EnemyManager.Instance.OnEnemyFinished(this);
    }

    private void EvaluateState()
    {
        if (!HasEndFlashed())
        {
            ChangeState(new FlashedState());
        }
        else if (!_pathNavigator.HasReachedDestination)
        {
			ChangeState(new DistractedState());
		}
        else
        {
			ChangeState(new NormalState());
		}
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
        return seq;
    }


    // Molotov-ed
    public Tween Burn()
    {
        Sequence seq = DOTween.Sequence();

        Node targetNode = _gridSensor.FindEscapeNode(_facingDirection);

        // Cannot find any walkable node, so die
        if (targetNode == null)
        {
            seq.Append(Die());
            return seq;
        }

        // Runaway
        Direction targetDir = GridMathUtility.GetDirectionFromTargetNode(_currentNode, targetNode);

        if (targetDir != _facingDirection)
            seq.Append(_enemyMovement.Rotate(targetDir, _actionDuration));

        seq.Append(_enemyMovement.Move(targetNode, _actionDuration));
        return seq;
    }

    // Decoy-ed
    public Tween HearNoise(Node noiseOrigin)
    {
        if (IsFlashed || _isDead)
            return null;

        _pathNavigator.SetDestination(_currentNode, noiseOrigin);
        if (_pathNavigator.HasReachedDestination) return null;

        Sequence seq = DOTween.Sequence();
        Direction targetDirection = GridMathUtility.GetDirectionFromTargetNode(_currentNode, _pathNavigator.NextNode);

        seq.Append(_enemyVisual.ShowQuestionIcon());
        seq.Append(_enemyMovement.Rotate(targetDirection, _actionDuration));

		//seq.OnComplete(() => { ChangeState(State.Distracted); });
		seq.OnComplete(() => { ChangeState(new DistractedState()); });
		return seq;
    }


    // Flash-ed
    public Tween GetFlashed(int duration)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
            {
                //ChangeState(State.Flashed);
                ChangeState(new FlashedState());
                _flashTurnsRemaining = Mathf.Max(_flashTurnsRemaining, duration);
                _pathNavigator.ClearPath();
            }
        );
        seq.Append(_enemyVisual.Wobble());
        seq.JoinCallback(() => { _enemyVisual.ShowStunIcon(); });
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

    public void SetBehavior(BaseEnemyBehavior newBahav){
        _currentBehavior = newBahav;
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