using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grid
{
    public class NodeManager : Singleton<NodeManager>
    {
        [Header("References")]
        [SerializeField] private Transform _cellContainer;
        [SerializeField] Node _nodePrefab;

        [SerializeField] private int _widthX;
        [SerializeField] private int _heightY;
        [SerializeField] private float _cellSize;

        [SerializeField] private List<Node> _allNodes = new();
        private Dictionary<Vector2Int, Node> _nodeGrid = new();
        public IReadOnlyList<Node> AllNodes => _allNodes;

        protected override void Awake()
        {
            base.Awake();
            RebuildNodeGrid();
        }

        public void GenerateMap(int width, int height, float size)
        {
            _widthX = width;
            _heightY = height;
            _cellSize = size;

            DeleteAllNodes();
            GenerateNodes();

            AssignNodeNeighbour();
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

        public bool TryGetNode(Vector2Int coord, out Node node)
        {
            return _nodeGrid.TryGetValue(coord, out node);
        }

        public List<Node> GetNodesInRange(Node centerNode, int range, bool includeCenter = false)
        {
            Vector2Int centerCoord = centerNode.Get2DCoordinate();
            var result = new List<Node>();


            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (!includeCenter && dx == 0 && dy == 0)
                        continue;

                    // Chebyshev distance check (square range)
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) > range)
                        continue;

                    Vector2Int coord = centerCoord + new Vector2Int(dx, dy);

                    if (_nodeGrid.TryGetValue(coord, out Node node))
                        result.Add(node);
                }
            }

            return result;
        }


        private void SpawnNode(int x, int y, float cellSize)
        {
            Vector3 localPos = new Vector3(x, 0, y) * cellSize;
            Node node;

#if UNITY_EDITOR
            node = (Node)PrefabUtility.InstantiatePrefab(_nodePrefab, _cellContainer);
            Undo.RegisterCreatedObjectUndo(node.gameObject, "Spawn Node");
#else
    // 1. Runtime Instantiation
    node = Instantiate(_nodePrefab, _cellContainer);
#endif

            node.transform.localPosition = localPos;

            node.gameObject.name = $"Node ({x}, {y})";
            node.Initialize(x, y, cellSize);

            _allNodes.Add(node);
            _nodeGrid.TryAdd(new Vector2Int(x, y), node);
        }


        public void AssignNodeNeighbour()
        {
            RebuildNodeGrid();

            Debug.Log("Assigning neighbors...");
            foreach (var node in _allNodes)
            {
                Vector2Int coord = node.Get2DCoordinate();

                if (_nodeGrid.TryGetValue(coord + Vector2Int.up, out Node northNode))
                {
                    node.AssignNeighbour(northNode, Direction.North);
                    northNode.AssignNeighbour(node, Direction.South);
                }

                if (_nodeGrid.TryGetValue(coord + Vector2Int.right, out Node eastNode))
                {
                    node.AssignNeighbour(eastNode, Direction.East);
                    eastNode.AssignNeighbour(node, Direction.West);
                }
            }

            Debug.Log("Neighbor assignment complete.");
        }


        public void DeleteAllNodes()
        {
            _allNodes.Clear();
            _nodeGrid.Clear();

            for (int i = _cellContainer.childCount - 1; i >= 0; i--)
            {
                GameObject child = _cellContainer.GetChild(i).gameObject;
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(child);
#else
            Destroy(child);
#endif
            }
        }

        public Node GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            RebuildNodeGrid();
            Vector3 localPos = _cellContainer.InverseTransformPoint(worldPosition);

            float x = localPos.x / _cellSize;
            float z = localPos.z / _cellSize;
            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(z)
            );

            Debug.Log($"Grid pos: {gridPos}");
            
            if (_nodeGrid.TryGetValue(gridPos, out Node node))
            {
                return node;
            }

            if (_nodeGrid == null || _nodeGrid.Count == 0)
            {
                Debug.LogError("Node grid is null!");
            }
            return null;
        }

        public void ReAssign(int width, int height, float size)
        {
            _widthX = width;
            _heightY = height;
            _cellSize = size;
        }
        public void RebuildNodeGrid()
        {
            if (_allNodes == null)
                _allNodes = new List<Node>();

            int removedCount = _allNodes.RemoveAll(n => n == null);
            if (removedCount > 0)
            {
                Debug.LogWarning($"Removed {removedCount} null nodes from manager cache.");
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }

            if (_nodeGrid != null && _nodeGrid.Count == _allNodes.Count && _nodeGrid.Count > 0)
                return;

            _nodeGrid = new Dictionary<Vector2Int, Node>();
            foreach (Node node in _allNodes)
            {
                Vector2Int coord = node.Get2DCoordinate();
                if (!_nodeGrid.ContainsKey(coord))
                {
                    _nodeGrid.Add(coord, node);
                }
                else
                {
                    Debug.LogError($"Duplicate Node detected at {coord}! Check map generation.");
                }
            }
        }
    }
}