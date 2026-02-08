using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn.EnemyBehaviors
{
    public class MovingBehavior : BaseEnemyBehavior
    {
        [Range(0.1f, 2f)] [SerializeField] private float _actionDuration = 1.0f;

        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();
            float duration = TurnManager.Instance.GlobalActionDuration * _actionDuration;

            Node firstNodeInFront = enemy.GetNodeInFront();
            if (firstNodeInFront != null && !firstNodeInFront.IsObstacle)
            {
                plan.Add(new MoveAction(firstNodeInFront, duration));

                Node secondNodeInFront = enemy.GetNodeInDirection(firstNodeInFront, enemy.CurrentFacingDirection);

                if (secondNodeInFront == null || secondNodeInFront.IsObstacle)
                {
                    Direction targetDirection = enemy.GetDirectionTurnAround();
                    plan.Add(new RotateAction(targetDirection, duration, 0.1f));
                }
            }
            else
            {
                Direction targetDirection = enemy.GetDirectionTurnAround();
                plan.Add(new RotateAction(targetDirection, duration));
            }

            return plan;
        }
    }
}