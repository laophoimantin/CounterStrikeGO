using System;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Pawn
{
    public abstract class PawnUnit : GridOccupant
    {
        [SerializeField] protected float _moveDuration = 1;
        protected bool _isDead = false;
        public bool IsDead => _isDead;
        public override bool OccupiesSpace => !_isDead;
        public abstract Sequence Terminate();
    }
}