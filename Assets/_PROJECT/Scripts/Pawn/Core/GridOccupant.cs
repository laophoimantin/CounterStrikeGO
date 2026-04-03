using UnityEngine;

public abstract class GridOccupant : MonoBehaviour
{
    [Header("Node References")]
    [Tooltip("Do not touch, this is for debugging purposes only")]
    [SerializeField] protected Node _currentNode;

    [Header("Visual References")]
    [SerializeField] protected GridUnitVisual _visual;
    public Node CurrentNode => _currentNode;

    public virtual bool OccupiesSpace => true;

    public void SetVisualOffset(Vector3 offset)
    {
        _visual.MoveOffset(offset);
    }
    
    public void ChangeNode(Node newNode)
    {
        if (newNode == null) return;

        if (_currentNode != null)
        {
            _currentNode.RemoveUnit(this);
        }

        _currentNode = newNode;
        _currentNode.AddUnit(this);
    }
    
    protected void UnAssignCurrentNode()
    {
        if (_currentNode != null)
        {
            _currentNode.RemoveUnit(this);
        }
        _currentNode = null;
    }
    
    protected void SnapToNode(Node node)
    {
        if (node == null) return;

        transform.position = node.WorldPos;
        _visual.SetPosition(transform.position);

        ChangeNode(node);
    }

}