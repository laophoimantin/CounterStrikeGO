using System.Collections.Generic;
using Core.TurnSystem;
using UnityEngine;

namespace Pawn
{
    public abstract class BaseEnemyBehavior: MonoBehaviour
    {
        [Range(0.1f, 2f)] [SerializeField] private float _actionDurationModifier = 1.0f;
        protected float Duration => TurnManager.Instance.GlobalActionDuration * _actionDurationModifier;

        // Always make sure there is something on the list for the enemy to do!!!!
        public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
    }
}