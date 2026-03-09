using System.Collections.Generic;
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

            bool hasMove = false;

            Node firstNodeInFront = enemy.GetNodeInFront();
            if (firstNodeInFront != null && firstNodeInFront.IsWalkable())
            {
                plan.Add(new MoveAction(firstNodeInFront));
                hasMove = true;
				Node secondNodeInFront = enemy.GetNodeInDirection(firstNodeInFront, enemy.CurrentFacingDirection);

                if (secondNodeInFront == null || !secondNodeInFront.IsWalkable())
                {
                    Direction targetDirection = enemy.GetDirectionTurnAround();
                    plan.Add(new RotateAction(targetDirection, 0.1f));
                }
            }
            else
            {
                Direction targetDirection = enemy.GetDirectionTurnAround();
                plan.Add(new RotateAction(targetDirection));

                if (!hasMove)
                {
                    hasMove = true;
                    Node nodeBehind = enemy.GetNodeInBack();
                    if (nodeBehind != null && nodeBehind.IsWalkable())
                    {
                        plan.Add(new MoveAction(nodeBehind));
                    }
                }
            }

            return plan;
        }
    }
}