using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelBuilderManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NodeManager _nodeManager;
    public NodeManager NodeManager => _nodeManager;

    [Header("Builder Tools")]
    [SerializeField] private Transform _cellContainer;
    [SerializeField] private Node _nodePrefab;
    
    [Header("Map Config")]
    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    private const float CELL_SIZE = 8;

    public void GenerateNodeMap()
    {
        DeleteMap(); 

        List<Node> newNodes = new List<Node>();

        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                Node newNode = SpawnNode(x, y, CELL_SIZE);
                newNodes.Add(newNode);
            }
        }

        _nodeManager.SetupGridData(_mapWidth, _mapHeight, CELL_SIZE, newNodes);

        AssignNodeNeighbors();
    }

    private Node SpawnNode(int x, int y, float cellSize)
    {
        Vector3 localPos = new Vector3(x, 0, y) * cellSize;
        Node node;

#if UNITY_EDITOR
        node = (Node)PrefabUtility.InstantiatePrefab(_nodePrefab, _cellContainer);
        Undo.RegisterCreatedObjectUndo(node.gameObject, "Spawn Node");
#else
        node = Instantiate(_nodePrefab, _cellContainer);
#endif

        node.transform.localPosition = localPos;
        node.gameObject.name = $"Node ({x}, {y})";
        node.Initialize(x, y, cellSize);

        return node;
    }
    public void DeleteMap()
    {
        for (int i = _cellContainer.childCount - 1; i >= 0; i--)
        {
            GameObject child = _cellContainer.GetChild(i).gameObject;
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(child);
#else
            Destroy(child);
#endif
        }
        _nodeManager.ClearGridData();
    }
    
    public void AssignNodeNeighbors()
    {
        Debug.Log("Assigning neighbors...");
        foreach (var node in _nodeManager.AllNodes)
        {
            int x = node.XValue;
            int y = node.YValue;

            if (_nodeManager.TryGetNode(x, y + 1, out Node northNode))
                node.AssignNeighbour(northNode, Direction.North);

            if (_nodeManager.TryGetNode(x + 1, y, out Node eastNode))
                node.AssignNeighbour(eastNode, Direction.East);
        }
        Debug.Log("Neighbor assignment complete.");
    }
}