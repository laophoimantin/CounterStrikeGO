using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Moving", menuName = "Behav/Moving", order = 3)]
public class MovingBehavior : StandardEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        Node targetNode = enemy.GetNodeInFront();
        Direction currentDir = enemy.CurrentFacingDirection;
        Direction moveDir = currentDir;

        if (targetNode == null || !targetNode.IsWalkable())
        {
            moveDir =  GridMathUtility.TurnAround(enemy.CurrentFacingDirection);
            targetNode = enemy.GetNodeInBack();
            baseList.Add(new RotateAction(moveDir));
        }

        if (targetNode != null && targetNode.IsWalkable())
        {
            baseList.Add(new MoveAction(targetNode));

            Node nodeAfterNext = targetNode.GetNodeInDirection(moveDir);

            if (nodeAfterNext == null || !nodeAfterNext.IsWalkable())
            {
                Direction reverseDir = (moveDir == currentDir) ? GridMathUtility.TurnAround(enemy.CurrentFacingDirection) : currentDir;
                baseList.Add(new RotateAction(reverseDir, 0.1f));
            }
        }
    }
}