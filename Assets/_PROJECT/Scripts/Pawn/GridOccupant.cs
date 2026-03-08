using Grid;
using UnityEngine;

public abstract class GridOccupant : MonoBehaviour
{
    [SerializeField] protected GridUnitVisual _visual;
    [SerializeField] protected Node _currentNode;

    public Node CurrentNode => _currentNode;

    public virtual bool IsActive => true;

    public void SetVisualOffset(Vector3 offset)
    {
        _visual.DoOffsetMove(offset);
    }
}