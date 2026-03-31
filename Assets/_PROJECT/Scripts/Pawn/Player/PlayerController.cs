using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PlayerController : PawnUnit
{
    private PlayerVisual _playerVisual;
    public PlayerVisual PlayerVisual => _playerVisual;

    [Header("References")]
    [SerializeField] private UnitCombat _unitCombat;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerUtilityHandler _utilityHandler;

    private bool _canAct = true;

    private Direction _tempMoveDirection = Direction.None;
    public bool HasUtility => _utilityHandler.HasItem;

    void OnEnable()
    {
        this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    void Awake()
    {
        if (_currentNode == null)
        {
            Debug.LogWarning($"{gameObject.name} has no node assigned!");
            return;
        }

        _playerVisual = _visual as PlayerVisual;
    }

    void Start()
    {
        if (_currentNode != null)
        {
            SnapToNode(_currentNode);
        }
    }

    // Turn System =========================================================================
    private void HandleTurnChanged(OnTurnChangedEvent eventData)
    {
        _canAct = (eventData.NewTurn == TurnType.PlayerPlanning);
        if (eventData.NewTurn == TurnType.PlayerPlanning && _tempMoveDirection != Direction.None)
            TryMoveTo(_tempMoveDirection);
    }

    // Actions ====================================================
    public void TryMoveTo(Direction direction)
    {
        _tempMoveDirection = direction;
        if (!_canAct || _utilityHandler.HasItem || _playerMovement.IsMoving) return;

        Node target = _currentNode.GetNodeInDirection(direction);

        if (target == null || !target.IsWalkable())
        {
            _tempMoveDirection = Direction.None;
            return;
        }

        _tempMoveDirection = Direction.None;
        StartAction();
        PlayMoveSequence(target);
    }

    private void PlayMoveSequence(Node targetNode)
    {
        _canAct = false;

        ChangeNode(targetNode);

        Sequence seq = DOTween.Sequence();

        _playerMovement.AppendMoveSequence(targetNode, seq, _actionDuration);
        _unitCombat.AppendAttackSequence(targetNode, seq);

        seq.OnComplete(() =>
        {
            _currentNode.TriggerEnter(this);
            FinishAction(ShouldEndTurn());
        });
    }


    private bool ShouldEndTurn()
    {
        return true;
    }

    private void StartAction()
    {
        this.SendEvent(new OnPlayerActionStartedEvent());
    }

    private void FinishAction(bool shouldEnd)
    {
        this.SendEvent(new OnPlayerActionFinishedEvent(shouldEnd));
    }

    public override Tween Die()
    {
        Sequence deathSequence = DOTween.Sequence();
        deathSequence.Append(_playerVisual.Wobble());
        deathSequence.Append(_playerVisual.FlyUp());
        deathSequence.OnComplete(() => { this.SendEvent(new OnPlayerDeadEvent()); });
        return deathSequence;
    }

    public void Input_TryUseUtility(Node targetNode)
    {
        if (!_canAct || !_utilityHandler.HasItem) return;
        StartAction();
        _utilityHandler.TryUseUtility(targetNode, (endsTurn) => FinishAction(endsTurn));
    }

    // On Click ====================================================================================
    public void OnPickedUp()
    {
        _playerVisual.PickUpAnim();
    }

    public void OnDropped()
    {
        _playerVisual.DropAnim();
    }

    private void SnapToNode(Node node)
    {
        if (node == null) return;

        transform.position = node.WorldPos;
        _playerVisual.SetPosition(transform.position);

        ChangeNode(node);
    }

    // Editor ====================================================================================

    #region Editor Methods

#if UNITY_EDITOR
    public void SetOrMoveNode(Direction? dir = null)
    {
        if (NodeManager.Instance == null)
        {
            var found = FindObjectOfType<NodeManager>();
            if (found == null) return;
        }

        Node newNode;

        if (dir.HasValue)
        {
            newNode = _currentNode.GetNodeInDirection(dir.Value);
        }
        else
        {
            newNode = FindObjectOfType<NodeManager>().GetNodeFromWorldPosition(transform.localPosition);
        }

        if (newNode == null)
        {
            Debug.LogWarning("No valid node found!");
            return;
        }

        // Logic
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