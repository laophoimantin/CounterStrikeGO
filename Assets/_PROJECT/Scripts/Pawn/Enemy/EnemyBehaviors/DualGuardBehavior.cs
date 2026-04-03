using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DualGuard", menuName = "Behav/DualGuard", order = 3)]
public class DualGuardBehavior : StandardEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        Direction behind = GridMathUtility.TurnAround(enemy.CurrentFacingDirection);
        if (enemy.GridSensor.ScanForEnemy(behind, _attackRange))
        {
            Node targetNode = enemy.GetNodeInBack();
            baseList.Add(new MoveAction(targetNode));
            baseList.Add(new AttackAction(targetNode));
        }
    }
}