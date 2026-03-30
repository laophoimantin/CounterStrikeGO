using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PlayerController : PawnUnit, IUtilityEquipper
{
    [Header("References")]
    private PlayerVisual _playerVisual;

    private bool _isMoving = false;
    private bool _canAct = true;

    private Direction _tempMoveDirection = Direction.None;
    private UtilityController _currentUtility;
    private bool _hasUtility = false;
    public bool HasUtility => _hasUtility;

    void OnEnable()
    {
        this.Subscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChanged);
    }

    void Start()
    {
        InitializeOnCurrentNode();
        _playerVisual = _visual as PlayerVisual;
    }

    private void InitializeOnCurrentNode()
    {
        if (_currentNode == null)
        {
            Debug.LogWarning($"{gameObject.name} has no node assigned!");
            return;
        }

        _currentNode.AddUnit(this);
        transform.position = _currentNode.WorldPos;
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
        if (!_canAct || _hasUtility || _isMoving) return;

        Node target = GetNodeInDirection(_currentNode, direction);

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
        _isMoving = true;
        _canAct = false;

        UpdateNodeData(targetNode);

        Sequence seq = DOTween.Sequence();

        seq.Append(_playerVisual.MoveTo(targetNode.WorldPos, _actionDuration));
        _playerVisual.TryAddWobble(seq);
        TryAttack(targetNode, seq);

        seq.OnComplete(() =>
        {
            _isMoving = false;
            _currentNode.TriggerEnter(this);
            FinishAction(ShouldEndTurn());
        });
    }

    private void TryAttack(Node targetNode, Sequence seq)
    {
        Tween combat = CombatResolver.ResolveAttackOnNode(targetNode, _team);

        if (combat != null)
            seq.Append(combat);
    }


    private bool ShouldEndTurn()
    {
        // if (_hasUtility)
        // {
        //     return false;
        // }

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

    private void UpdateNodeData(Node newNode)
    {
        if (newNode == null) return;

        if (_currentNode != null)
        {
            _currentNode.RemoveUnit(this);
        }

        _currentNode = newNode;
        _currentNode.AddUnit(this);
    }

    public override Tween Die()
    {
        Sequence deathSequence = DOTween.Sequence();
        deathSequence.Append(_playerVisual.Wobble());
        deathSequence.Append(_playerVisual.FlyUp());
        deathSequence.OnComplete(() => { this.SendEvent(new OnPlayerDeadEvent()); });
        return deathSequence;
    }

    public void EquipUtility(UtilityController newUtility)
    {
        _currentUtility = newUtility;
        _hasUtility = true;
        _playerVisual.SetUsingUtilityState(_hasUtility);
    }

    public void TryUseUtility(Node targetNode)
    {
        if (!_canAct || !_hasUtility) return;

        List<Node> validNodes = NodeManager.Instance.GetNodesInRange(_currentNode, _currentUtility.ThrowRange);
        if (validNodes.Contains(targetNode))
        {
            StartAction();
            UseUtility(targetNode);
        }
    }

    private void UseUtility(Node targetNode)
    {
        bool endsTurn = _currentUtility.EndsTurn;
        _playerVisual.Wobble();
        _currentUtility.Throw(targetNode, () => FinishAction(endsTurn));
        UnEquipItem();
        _playerVisual.SetUsingUtilityState(_hasUtility);
    }

    private void UnEquipItem()
    {
        _currentUtility = null;
        _hasUtility = false;
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
            newNode = GetNodeInDirection(_currentNode, dir.Value);
        }
        else
        {
            // Snap to nearest node
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

        _currentNode = newNode;
        _currentNode.AddUnit(this);

        // Visual Snap
        transform.position = _currentNode.transform.position;

        EditorUtility.SetDirty(this);
    }

    private Node GetNodeInDirection(Node node, Direction dir)
    {
        if (node == null) return null;
        return dir switch
        {
            Direction.North => node.NorthNode,
            Direction.South => node.SouthNode,
            Direction.East => node.EastNode,
            Direction.West => node.WestNode,
            _ => null
        };
    }
#endif

    #endregion
}