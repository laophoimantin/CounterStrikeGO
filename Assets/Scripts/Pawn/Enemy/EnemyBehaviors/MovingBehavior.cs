using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Moving", menuName = "Behav/Moving", order = 3)]
    public class MovingBehavior : BaseEnemyBehavior
    {
        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();
            Node firstNodeInFront = enemy.GetNodeInFront();
            if (firstNodeInFront != null && firstNodeInFront.IsWalkable())
            {
                plan.Add(new MoveAction(firstNodeInFront, Duration));

                Node secondNodeInFront = enemy.GetNodeInDirection(firstNodeInFront, enemy.CurrentFacingDirection);

                if (secondNodeInFront == null || !secondNodeInFront.IsWalkable())
                {
                    Direction targetDirection = enemy.GetDirectionTurnAround();
                    plan.Add(new RotateAction(targetDirection, Duration, 0.1f));
                }
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