using System;
using UnityEngine;

public enum TeamSide
{
    Player,
    Enemy
}

namespace Pawn
{
    public abstract class GridUnit : MonoBehaviour
    {
        public abstract TeamSide Team { get; }
        public abstract void Die(Action onDeathComplete = null);
    }
}