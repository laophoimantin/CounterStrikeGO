using System.Collections.Generic;
using UnityEngine;

public class PathNavigator : MonoBehaviour
{
    private List<Node> _currentPath = new();

    public bool HasPath => _currentPath != null && _currentPath.Count > 0;
    
    public bool HasReachedDestination => !HasPath || _currentPath.Count <= 1;

    public Node NextNode => HasPath && _currentPath.Count > 1 ? _currentPath[1] : null;
    public Node UpcomingNode => HasPath && _currentPath.Count > 2 ? _currentPath[2] : null;

    public void SetDestination(Node startNode, Node targetNode)
    {
        _currentPath = AstarPathfinder.FindPath(startNode, targetNode);
    }

    public void AdvancePath()
    {
        if (HasPath)
        {
            _currentPath.RemoveAt(0);
        }
    }

    public void ClearPath()
    {
        _currentPath?.Clear();
    }
}