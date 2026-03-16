using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Moving", menuName = "Behav/Moving", order = 3)]
    public class MovingBehavior : BaseEnemyBehavior
    {
        protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
        {
            bool hasMove = false;

            Node firstNodeInFront = enemy.GetNodeInFront();
            if (firstNodeInFront != null && firstNodeInFront.IsWalkable())
            {
                baseList.Add(new MoveAction(firstNodeInFront));
                hasMove = true;
				Node secondNodeInFront = enemy.GetNodeInDirection(firstNodeInFront, enemy.CurrentFacingDirection);

                if (secondNodeInFront == null || !secondNodeInFront.IsWalkable())
                {
                    Direction targetDirection = enemy.GetDirectionTurnAround();
                    baseList.Add(new RotateAction(targetDirection, 0.1f));
                }
            }
            else
            {
                Direction targetDirection = enemy.GetDirectionTurnAround();
                baseList.Add(new RotateAction(targetDirection));

                if (!hasMove)
                {
                    hasMove = true;
                    Node nodeBehind = enemy.GetNodeInBack();
                    if (nodeBehind != null && nodeBehind.IsWalkable())
                    {
                        baseList.Add(new MoveAction(nodeBehind));
                    }
                }
            }
        }
    }
}