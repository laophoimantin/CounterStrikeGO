using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turnning", menuName = "Behav/Turnning", order = 2)]
public class TurningBehavior : BaseEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        Direction targetDirection = GridMathUtility.TurnAround(enemy.CurrentFacingDirection);
        baseList.Add(new RotateAction(targetDirection));
    }
}