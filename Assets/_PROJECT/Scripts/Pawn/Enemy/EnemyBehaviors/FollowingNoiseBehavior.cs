using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Noise", menuName = "Behav/Noise", order = 10)]
public class FollowingNoiseBehavior : BaseEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        if (enemy.HasReachedNoiseDestination())
        {
            return;
        }

        var nextNode = enemy.NextNode;
        baseList.Add(new MoveAction(nextNode));

        var upcoming = enemy.UpcomingNode;
        if (upcoming != null)
        {
            Direction dir = enemy.GetDirectionFromTargetNode(nextNode, upcoming);
            baseList.Add(new RotateAction(dir));
        }
    }
}