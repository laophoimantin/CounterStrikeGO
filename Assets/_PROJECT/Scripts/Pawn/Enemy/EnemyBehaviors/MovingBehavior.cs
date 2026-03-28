using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Moving", menuName = "Behav/Moving", order = 3)]
    public class MovingBehavior : BaseEnemyBehavior
    {
        // protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
        // {
        //     Node firstNodeInFront = enemy.GetNodeInFront();
        //
        //     if (firstNodeInFront != null && firstNodeInFront.IsWalkable())
        //     {
        //         baseList.Add(new MoveAction(firstNodeInFront));
        //         Node nextNode = enemy.GetNodeInDirection(firstNodeInFront, enemy.CurrentFacingDirection);
        //         if (nextNode == null || !nextNode.IsWalkable())
        //         {
        //             Direction targetDirection = enemy.GetDirectionTurnAround();
        //             baseList.Add(new RotateAction(targetDirection, 0.1f));
        //         }
        //     }
        //     else
        //     {
        //         Direction targetDirection = enemy.GetDirectionTurnAround();
        //         baseList.Add(new RotateAction(targetDirection));
        //
        //         Node nodeBehind = enemy.GetNodeInBack();
        //         if (nodeBehind != null && nodeBehind.IsWalkable())
        //         {
        //             baseList.Add(new MoveAction(nodeBehind));
        //             Node secondNodeBehind = enemy.GetNodeInDirection(nodeBehind, targetDirection);
        //
        //             if (secondNodeBehind == null || !secondNodeBehind.IsWalkable())
        //             {
        //                 Direction turnDirection = enemy.CurrentFacingDirection;
        //                 baseList.Add(new RotateAction(turnDirection));
        //             }
        //         }
        //     }
        // }
        protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
        {
            Node targetNode = enemy.GetNodeInFront();
            Direction currentDir = enemy.CurrentFacingDirection;
            Direction moveDir = currentDir;

            if (targetNode == null || !targetNode.IsWalkable())
            {
                moveDir = enemy.GetDirectionTurnAround(); 
                targetNode = enemy.GetNodeInBack();      
                baseList.Add(new RotateAction(moveDir)); 
            }

            if (targetNode != null && targetNode.IsWalkable())
            {
                baseList.Add(new MoveAction(targetNode));

                Node nodeAfterNext = enemy.GetNodeInDirection(targetNode, moveDir);
        
                if (nodeAfterNext == null || !nodeAfterNext.IsWalkable())
                {
                    Direction reverseDir = (moveDir == currentDir) ? enemy.GetDirectionTurnAround() : currentDir;
                    baseList.Add(new RotateAction(reverseDir, 0.1f)); 
                }
            }
        }
    }
}