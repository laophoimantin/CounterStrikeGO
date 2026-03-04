using System;
using Grid;
using UnityEngine;

namespace Pawn
{
    public abstract class GridUnit : MonoBehaviour
    {
        [SerializeField] protected GridUnitVisual _visual;
        [SerializeField] protected Node _currentNode;
        public Node CurrentNode => _currentNode;
        protected bool _isDead = false;
        public bool IsDead => _isDead;

        public abstract void Terminate(Action onDeathComplete = null);

        public virtual void SetVisualOffset(Vector3 localOffset)
        {
            _visual.DoOffsetMove(localOffset);
        }
    }
}