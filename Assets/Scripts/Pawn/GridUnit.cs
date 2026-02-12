using System;
using UnityEngine;
using DG.Tweening;


public enum TeamSide
{
    Player,
    Enemy
}

namespace Pawn
{
    public abstract class GridUnit : MonoBehaviour
    {
        public abstract Transform VisualModel { get; }
        public abstract TeamSide Team { get; }
        public abstract void Die(Action onDeathComplete = null);

        public virtual void SetVisualOffset(Vector3 localOffset)
        {
            VisualModel.DOLocalMove(localOffset, 0.3f).SetEase(Ease.OutQuad);
        }
    }
}