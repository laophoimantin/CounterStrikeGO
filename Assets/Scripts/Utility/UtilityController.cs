using System;
using Grid;
using Pawn;
using System.Collections;
using UnityEngine;

public abstract class UtilityController : GridUnit
{
    private PlayerController _owner;
    
    [SerializeField] private int _throwRange = 1;
    public int ThrowRange => _throwRange;
    
    [Header("References")]
    [SerializeField] private Collider _collider;
    private UtilityVisual _utilityVisual;

    [SerializeField] private bool _endsTurn = true;
    public bool EndsTurn => _endsTurn;

    void Start()
    {
        if (_currentNode != null)
        {
            _currentNode.AddUnit(this);
            _currentNode.AddUtility(this);
        }

        _utilityVisual = _visual as UtilityVisual;
        if (_utilityVisual == null)
            Debug.Log("VISUALLLLLL!");
    }
       
    public void OnPickUp(PlayerController player)
    {
        if (_currentNode != null)
        {
            _currentNode.RemoveUnit(this);
            _currentNode.RemoveUtility();
            _currentNode = null;
        }
        
        _owner = player;
        _collider.enabled = false;
        _utilityVisual.SwitchToFlyingMode(player.transform.position);
    }

    public void Throw(Node targetNode, Action onComplete)
    {
        StartCoroutine(ThrowRoutine(targetNode, onComplete));
    }

    private IEnumerator ThrowRoutine(Node targetNode, Action onComplete)
    {

        yield return _utilityVisual.AnimateThrow(targetNode.WorldPos, 0.5f);
        _utilityVisual.HideUtitlityModel();
        yield return OnLanded(targetNode);
        Terminate(onComplete);
    }

    public override void Terminate(Action onDeathComplete = null)
    {
        Destroy(gameObject);
        onDeathComplete?.Invoke();
    }
    
    protected abstract IEnumerator OnLanded(Node targetNode);

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