using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeManager : Singleton<NodeManager>
{
    [Header("References")]
    [SerializeField] private GridVisualizer _gridVisualizer;
    
    [Header("Grid Data")]
    [SerializeField] private int _widthX;
    [SerializeField] private int _heightY;
    [SerializeField] private float _cellSize;

    [SerializeField] private List<Node> _allNodes = new();
    public IReadOnlyList<Node> AllNodes => _allNodes;


    void Start()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        _gridVisualizer.DrawAllConnections();
    }

    public void SetupGridData(int width, int height, float size, List<Node> generatedNodes)
    {
        _widthX = width;
        _heightY = height;
        _cellSize = size;
        _allNodes = generatedNodes;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this); 
#endif
    }
    
    public void ClearGridData()
    {
        _widthX = 0;
        _heightY = 0;
        _allNodes.Clear();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    
    // ===============================================================
    private int ToIndex(int x, int y)
    {
        return y * _widthX + x; 
    }

    public bool TryGetNode(int x, int y, out Node node)
    {
        node = null;
        if (x < 0 || x >= _widthX || y < 0 || y >= _heightY)
        {
            return false;
        }

        int index = ToIndex(x, y);

        if (index < 0 || index >= _allNodes.Count)
        {
            return false;
        }

        node = _allNodes[index];
        
        return node != null; 
    }

    public List<Node> GetNodesInRange(Node centerNode, int range, bool includeCenter = false, bool ignoreObstacles = true)
    {
        Vector2Int centerCoord = centerNode.Get2DCoordinate();
        var result = new List<Node>();

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                if (!includeCenter && dx == 0 && dy == 0) continue;
     
                int checkX = centerCoord.x + dx;
                int checkY = centerCoord.y + dy;

                if (TryGetNode(checkX, checkY, out Node node))
                {
                    if (ignoreObstacles && node.IsObstacle) 
                        continue;
                    result.Add(node);
                }
            }
        }
        return result;
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / _cellSize);
        int y = Mathf.RoundToInt(worldPosition.z / _cellSize); 

        if (TryGetNode(x, y, out Node node))
        {
            return node;
        }

        return null;
    }
}