using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyBehavior : ScriptableObject
{
    public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
}