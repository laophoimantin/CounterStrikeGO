using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plan the sequence of actions the enemy will take when it's their turn
/// </summary>
public abstract class BaseEnemyBehavior : ScriptableObject
{
    public virtual int ExecutionPriority => 0;
    public abstract List<BaseEnemyAction> PlanActions(EnemyController enemy);

    public virtual void OnStart(EnemyController enemy)
    {
    }

    public virtual void OnEnter(EnemyController enemy)
    {
        
    }
    public virtual void OnExit(EnemyController enemy)
    {
        
    }
}