using System;
using System.Collections.Generic;
using System.Drawing;
using Characters.Enemy;
using Core.Patterns;
using UnityEngine;

namespace Grid
{
    public class NodeManager : Singleton<NodeManager>
    {
        [SerializeField] private int _widthX;
        [SerializeField] private int _heightY;
        [SerializeField] private float _cellSize;
        [SerializeField] private Transform _cellContainer;
        [SerializeField] Node _nodePrefab;

        [SerializeField] private List<Node> _allNodes = new();
        private Dictionary<Vector2Int, Node> _nodeGrid = new();

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
        }

        public void GenerateMap(int width, int height, float size)
        {
            _widthX = width;
            _heightY = height;
            _cellSize = size;

            DeleteAllNodes();
            GenerateNodes();
        }

        private void GenerateNodes()
        {
            for (int i = 0; i < _widthX; i++)
            {
                for (int j = 0; j < _heightY; j++)
                {
                    SpawnNode(i, j, _cellSize);
                }
            }
        }

        private void SpawnNode(int x, int y, float cellSize)
        {
            Vector3 worldPos = new Vector3(x, 0, y) * cellSize;
            Node node = Instantiate(_nodePrefab, worldPos, Quaternion.identity, _cellContainer) as Node;
            node.gameObject.name = $"Node ({x}, {y})";
            node.Initialize(x, y, worldPos, cellSize);
            _allNodes.Add(node);

            _nodeGrid.TryAdd(new Vector2Int(x, y), node);
        }

        public void AssignNodeNeighbour()
        {
            Debug.Log("Assigning neighbors...");
            foreach (var node in _allNodes)
            {
                Vector2Int coord = node.Get2DCoordinate();

                if (_nodeGrid.TryGetValue(coord + Vector2Int.up, out Node northNode))
                {
                    node.AssignNeighbour(northNode, Direction.North);
                }
                if (_nodeGrid.TryGetValue(coord + Vector2Int.right, out Node eastNode))
                {
                    node.AssignNeighbour(eastNode, Direction.East);
                }
            }

            Debug.Log("Neighbor assignment complete.");
        }

        public void DeleteAllNodes()
        {
            _allNodes.Clear();
            _nodeGrid.Clear();
            while (_cellContainer.childCount > 0)
            {
                DestroyImmediate(_cellContainer.GetChild(0).gameObject);
            }
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            RebuildNodeGrid();

            float x = worldPosition.x / _cellSize;
            float y = worldPosition.z / _cellSize;
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(y)
            );
            if (_nodeGrid.TryGetValue(gridPos, out Node node))
            {
                return node;
            }

            return null;
        }
        
        
        
        // Editor ============================================================================================
        public void RebuildNodeGrid()
        {
            if (_nodeGrid.Count > 0)
                return;
            
            Debug.Log("Rebuilding node grid...");
            if (_allNodes == null || _allNodes.Count == 0)
            {
                Debug.LogWarning("Cannot rebuild node grid: no nodes available.");
                _nodeGrid.Clear();
                return;
            }
            
            _nodeGrid.Clear();
            foreach (Node node in _allNodes)
            {
                Vector2Int coordinate = node.Get2DCoordinate();
                _nodeGrid.TryAdd(coordinate, node);
            }
            Debug.Log("Node grid rebuild complete.");
        }
    }
}