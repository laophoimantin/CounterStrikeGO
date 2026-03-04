using System.Collections.Generic;
using Core.TurnSystem;
using UnityEngine;

namespace Pawn
{
    public abstract class BaseEnemyBehavior : ScriptableObject
    {
        [Range(0.1f, 2f)] [SerializeField] protected float _actionDurationModifier = 1.0f;
        protected float Duration => TurnManager.Instance.GlobalActionDuration * _actionDurationModifier;

        // Always make sure there is something on the list for the enemy to do!!!!
        public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
    }
}