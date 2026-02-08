using System.Collections.Generic;
using UnityEngine;

namespace Pawn
{
    public abstract class BaseEnemyBehavior: MonoBehaviour
    {
        // Always make sure there is something on the list for the enemy to do!!!!
        public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
    }
}