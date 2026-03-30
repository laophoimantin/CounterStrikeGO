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
}