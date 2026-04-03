using System.Collections.Generic;
using UnityEngine;

public abstract class StandardEnemyBehavior : BaseEnemyBehavior
{
    [SerializeField] protected int _attackRange = 1;

    public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
    {
        var plan = new List<BaseEnemyAction>();

        if (enemy.GridSensor.ScanForEnemy(enemy.CurrentFacingDirection, _attackRange))
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