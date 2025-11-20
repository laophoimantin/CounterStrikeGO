using System.Collections;
using System.Collections.Generic;
using Characters.Enemy.EnemyActions;
using UnityEngine;

namespace Characters.Enemy.EnemyBehaviors
{
    public abstract class BaseEnemyBehavior: MonoBehaviour
    {
        // Always make sure there is something on the list for the enemy to do!!!!
        public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
    }
}