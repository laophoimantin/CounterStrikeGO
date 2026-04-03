using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyBehavior : ScriptableObject
{
    public virtual int ExecutionPriority => 0;
    public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);
}