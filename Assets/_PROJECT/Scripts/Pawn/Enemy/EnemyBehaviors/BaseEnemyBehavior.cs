using System.Collections.Generic;
using Core.TurnSystem;
using UnityEngine;

namespace Pawn
{
    public abstract class BaseEnemyBehavior : ScriptableObject
    {
        // Always make sure there is something on the list for the enemy to do!!!!
        public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
    }
}