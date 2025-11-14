using System.Collections.Generic;
using System.Drawing;
using Core.Patterns;
using UnityEngine;

namespace Grid
{
    public class NodeManager : Singleton<NodeManager>
    {


        #region Private Fields

        [SerializeField] private int _widthX;
        [SerializeField] private int _heightY;
        [SerializeField] private float _cellSize;
        private int[,] _nodeArray;

        [SerializeField] private Transform _cellContainer;

        [SerializeField] Node _nodePrefab;

        private List<Node> _allNodes = new();
        private Dictionary<Vector2Int, Node> _nodeGrid = new();
        #endregion

        #region Public Fields


        #endregion


        protected override void Awake()
        {
            base.Awake();

        }

        private void Start()
        {

        }

        public void GenerateMap(int width, int height, float size)
        {
            _widthX = width;
            _heightY = height;
            _cellSize = size;
            _nodeArray = new int[_widthX, _heightY];
            GenerateNode();
        }

        private void GenerateNode()
        {
            _allNodes.Clear();
            _nodeGrid.Clear();
            while (_cellContainer.childCount > 0)
            {
                Destroy(_cellContainer.GetChild(0).gameObject);
            }




            for (int i = 0; i < _heightY; i++)
            {
                for (int j = 0; j < _widthX; j++)
                {
                    SpawnNode(j, i, _cellSize);
                }
            }
        }

        private void SpawnNode(int x, int y, float cellSize)
        {
            Vector3 pos = new Vector3(x, 0, y) * cellSize;
            Node node = Instantiate(_nodePrefab, pos, Quaternion.identity, _cellContainer) as Node;
            node.AssignValue(x, y, cellSize, pos);
            _allNodes.Add(node);

            if (_nodeGrid[node.GetCoordinate()] == null)
                _nodeGrid[node.GetCoordinate()] = node;

        }
    }
}