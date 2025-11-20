using System.Collections.Generic;
using Characters.Enemy.EnemyActions;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Characters.Enemy.EnemyBehaviors
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
                    Quaternion targetRotation = enemy.GetRotationTurnAround();
                    plan.Add(new RotateAction(targetRotation, duration, 0.1f));
                }
            }
            else
            {
                Quaternion targetRotation = enemy.GetRotationTurnAround();
                plan.Add(new RotateAction(targetRotation, duration));
            }

            return plan;
        }
    }
}