using System.Collections.Generic;
using Core.Patterns;
using UnityEngine;

namespace Grid
{
    public class GridManager : Singleton<GridManager>
    {

        #region Private Fields

        private List<OldNode> _allNodes = new();

        #endregion

        #region Public Fields

        public List<OldNode> AllNodes => _allNodes;

        #endregion


        protected override void Awake()
        {
            base.Awake();
            _allNodes.AddRange(FindObjectsOfType<OldNode>());
        }
    }
}