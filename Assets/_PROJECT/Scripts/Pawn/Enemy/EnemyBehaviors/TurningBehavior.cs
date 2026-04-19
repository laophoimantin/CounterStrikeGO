using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate in place
/// </summary>
[CreateAssetMenu(fileName = "Turnning", menuName = "Behav/Turnning", order = 2)]
public class TurningBehavior : StandardEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        Direction targetDirection = GridMathUtility.TurnAround(enemy.CurrentFacingDirection);
        baseList.Add(new RotateAction(targetDirection));
    }
}