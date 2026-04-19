using UnityEngine;

/// <summary>
/// Special node (Exit, spawn objective)
/// </summary>
public abstract class BaseNodeFeature : ScriptableObject
{
    private Node _currentNode;

    public virtual void Initialize(Node owner)
    {
        _currentNode = owner;
    }

    public abstract void OnEnter(PawnUnit pawnUnit);
}