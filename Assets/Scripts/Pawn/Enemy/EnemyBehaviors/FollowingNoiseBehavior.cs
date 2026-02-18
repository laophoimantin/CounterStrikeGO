using Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Noise", menuName = "Behav/Noise", order = 10)]
    public class FollowingNoiseBehavior : BaseEnemyBehavior
    {
        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();


            if (enemy.HasReachedNoiseDestination())
            {
                return plan;
            }

            var nextNode = enemy.NextNode;
            plan.Add(new MoveAction(nextNode, Duration));


            var upcoming = enemy.UpcomingNode;
            if (upcoming != null)
            {
                Direction dir = enemy.GetDirectionFromTargetNode(nextNode, upcoming);
                plan.Add(new RotateAction(dir, Duration));
            }
            return plan;
        }

     
    }
}