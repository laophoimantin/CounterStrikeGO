using System;
using Grid;
using UnityEngine;


public enum TeamSide
{
    None,
    Player,
    Enemy
}

namespace Pawn
{
    public abstract class GridUnit : MonoBehaviour
    {
        [SerializeField] protected GridUnitVisual _visual;
        [SerializeField] protected Node _currentNode;
        public abstract TeamSide Team { get; }
        public abstract void Die(Action onDeathComplete = null);

        public virtual void SetVisualOffset(Vector3 localOffset)
        {
            _visual.DoOffsetMove(localOffset);
        }
    }
}