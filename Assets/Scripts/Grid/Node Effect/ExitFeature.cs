using Pawn;
using Core;
using Grid;
using UnityEngine;

namespace Grid
{
    public class ExitFeature : MonoBehaviour, INodeFeature
    {
        private Node _node;

        public void Initialize(Node owner)
        {
            _node = owner;
        }
        public void OnEnter(GridUnit unit)
        {
            if (unit is PlayerController)
            {
                GameManager.Instance.CheckExitWinCondition();
            }
        }
    }
}