using System;
using Grid;
using Pawn;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class UtilityController : GridOccupant
{
    public override bool IsActive => true;

    [Header("Settings")]
    [SerializeField] private int _throwRange = 1;
    [SerializeField] private bool _endsTurn = true;
    public int ThrowRange => _throwRange;
    public bool EndsTurn => _endsTurn;

    [Header("References")]
    [SerializeField] private Collider _collider;

    private UtilityVisual _utilityVisual;

    void Awake()
    {
        _utilityVisual = _visual as UtilityVisual;

        if (_utilityVisual == null)
            Debug.LogError($"{name} requires UtilityVisual.");
    }

    void Start()
    {
        RegisterToNode();
    }

    private void RegisterToNode()
    {
        if (_currentNode == null) return;

        _currentNode.AddUnit(this);
        _currentNode.AddUtility(this);
    }

    private void UnregisterFromNode()
    {
        if (_currentNode == null) return;

        _currentNode.RemoveUnit(this);
        _currentNode.RemoveUtility();

        _currentNode = null;
    }

    public void OnPickUp(PlayerController player)
    {
        UnregisterFromNode();

        _collider.enabled = false;
        _utilityVisual.SwitchToFlyingMode(player.transform.position);
    }

    public void Throw(Node targetNode, Action onComplete)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_utilityVisual.GetThrowSequence(targetNode.WorldPos));
        AppendIfExists(sequence, _utilityVisual.GetLandedAnim());
        sequence.AppendCallback(() => _utilityVisual.HideUtilityModel());
        AppendIfExists(sequence, GetOnLandedSequence(targetNode));

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }
    private void AppendIfExists(Sequence seq, Sequence toAppend)
    {
        if (toAppend != null)
            seq.Append(toAppend);
    }

    protected abstract Sequence GetOnLandedSequence(Node targetNode);

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
            newNode = FindObjectOfType<NodeManager>().GetNodeFromWorldPosition(transform.position);
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
            _currentNode.RemoveUtility();
            UnityEditor.EditorUtility.SetDirty(_currentNode);
        }

        _currentNode = newNode;
        _currentNode.AddUnit(this);
        _currentNode.AddUtility(this);

        // Visual Snap
        transform.position = _currentNode.transform.position;

        UnityEditor.EditorUtility.SetDirty(this);
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