using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyBehavior : ScriptableObject
{
    // Always make sure there is something on the list for the enemy to do!!!!
    public List<BaseEnemyAction> PlanActions(EnemyController enemy)
    {
        var plan = new List<BaseEnemyAction>();

        if (enemy.ScanForPlayerInFront(1))
        {
            Node targetNode = enemy.GetNodeInFront();
            plan.Add(new MoveAction(targetNode));
            return plan;
        }

        CustomActions(plan, enemy);
        return plan;
    }

    protected abstract void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy);
}