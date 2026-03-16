using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Turnning", menuName = "Behav/Turnning", order = 2)]
    public class TurningBehavior : BaseEnemyBehavior
    {
        protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
        {
            Direction targetDirection = enemy.GetDirectionTurnAround();
            baseList.Add(new RotateAction(targetDirection));
        }
    }
}