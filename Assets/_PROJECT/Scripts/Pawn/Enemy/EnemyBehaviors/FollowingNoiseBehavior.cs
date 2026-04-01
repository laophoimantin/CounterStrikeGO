using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Noise", menuName = "Behav/Noise", order = 10)]
public class FollowingNoiseBehavior : BaseEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        var navigator = enemy.PathNavigator;
        var nextNode = navigator.NextNode;
        if (nextNode == null)
        {
            return;
        }

        baseList.Add(new MoveAction(nextNode));

        var upcoming = navigator.UpcomingNode;
        if (upcoming != null)
        {
            Direction dir = GridMathUtility.GetDirectionFromTargetNode(nextNode, upcoming);
            baseList.Add(new RotateAction(dir));
        }
    }
}