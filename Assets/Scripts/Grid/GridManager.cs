using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;

namespace Grid
{
    public class GridManager : Singleton<GridManager>
    {

        #region Private Fields

        private List<Node> _allNodes = new();

        #endregion

        #region Public Fields

        public List<Node> AllNodes => _allNodes;

        #endregion


        protected override void Awake()
        {
            base.Awake();
            _allNodes.AddRange(FindObjectsOfType<Node>());
        }

        public Node GetNodeFromPosition(Vector3 position)
        {
            foreach (var node in _allNodes)
            {
                if (Vector3.Distance(node.transform.position, position) < 0.1f)
                    return node;
            }

            return null;
        }
    }
}