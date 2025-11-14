using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;

namespace Grid
{
    public class NodeManager : Singleton<NodeManager>
    {

        #region Private Fields

        private List<Node> _allNodes = new();
        private Dictionary<Vector2Int, Node> _nodeGrid = new();
        #endregion

        #region Public Fields

        public List<Node> AllNodes => _allNodes;

        #endregion


        protected override void Awake()
        {
            base.Awake();
            _allNodes.AddRange(FindObjectsOfType<Node>());
        }
        
        public void RegisterNodes()
        {
            _nodeGrid.Clear();
            foreach (Node node in _allNodes)
            {
                // Round the position to the nearest integer
                Vector2Int gridPos = new Vector2Int(
                    Mathf.RoundToInt(node.transform.position.x),
                    Mathf.RoundToInt(node.transform.position.z) 
                );
            
                _nodeGrid[gridPos] = node;
            
                // node.gridPosition = gridPos; 
            }
            Debug.Log("Registered " + _nodeGrid.Count + " nodes.");
        }

        public Node GetNodeAtPosition(Vector2Int pos)
        {
            _nodeGrid.TryGetValue(pos, out Node node);
            return node;
        }
    }
}