using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public abstract class UtilityController : GridOccupant, IPickupable
{
    public override bool OccupiesSpace => true;

    [SerializeField] private Team _team;

    [Header("Settings")]
    [SerializeField] private int _throwRange = 1;
    [SerializeField] private bool _endsTurn = true; // Decide if after using this utility, the next turn is still player turn or enemy turn
    public int ThrowRange => _throwRange;
    public bool EndsTurn => _endsTurn;

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
        _currentNode.AddItem(this);
    }

    private void UnregisterFromNode()
    {
        if (_currentNode == null) return;

        _currentNode.RemoveUnit(this);
        _currentNode.RemoveItem();

        _currentNode = null;
    }

    public void OnPickUpBy(PawnUnit picker)
    {
        IUtilityEquipper receiver = picker.GetComponent<IUtilityEquipper>();
        if (receiver != null)
        {
            receiver.EquipUtility(this);
            UnregisterFromNode();
            _utilityVisual.SwitchToFlyingMode(picker.transform.position);
        }
    }

    public void Throw(Node targetNode, Action onComplete)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_utilityVisual.GetThrowSequence(targetNode.WorldPos));
        AppendIfExists(sequence, _utilityVisual.GetLandedAnim());
        sequence.AppendCallback(() => _utilityVisual.HideUtilityModel());
        AppendIfExists(sequence, GetOnLandedSequence(targetNode, _team));

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    private void AppendIfExists(Sequence seq, Tween toAppend)
    {
        if (toAppend != null)
        {
            seq.Append(toAppend);
        }
    }

    protected abstract Tween GetOnLandedSequence(Node targetNode, Team team);


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
            _currentNode.RemoveItem();
            EditorUtility.SetDirty(_currentNode);
        }

        _currentNode = newNode;
        RegisterToNode();

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