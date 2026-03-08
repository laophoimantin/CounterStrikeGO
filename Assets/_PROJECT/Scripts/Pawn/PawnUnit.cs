using System;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Pawn
{
    public abstract class PawnUnit : GridOccupant
    {
        protected bool _isDead = false;
        public bool IsDead => _isDead;
        public override bool IsActive => !_isDead;
        public abstract Sequence Terminate();
    }
}