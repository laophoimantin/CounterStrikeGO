using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn.EnemyBehaviors
{
    public class TurningBehavior : BaseEnemyBehavior
    {
        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();
            if (enemy.ScanForPlayerInFront(1))
            {
                Node targetNode = enemy.GetNodeInFront();
                plan.Add(new MoveAction(targetNode, Duration));
            }
            else
            {
                Direction targetDirection = enemy.GetDirectionTurnAround();
                plan.Add(new RotateAction(targetDirection, Duration));
            }
            return plan;
        }
    }
}