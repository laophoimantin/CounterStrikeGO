using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Move in a straight line, turn around if the next node is blocked
/// </summary>
[CreateAssetMenu(fileName = "Moving", menuName = "Behav/Moving", order = 3)]
public class MovingBehavior : StandardEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        Direction currentDir = enemy.CurrentFacingDirection;
        Direction moveDir = currentDir;
        Node targetNode = enemy.GetNodeInFront();

        if (targetNode == null || !targetNode.IsWalkable())
        {
            moveDir = GridMathUtility.TurnAround(currentDir);
            targetNode = enemy.GetNodeInBack();
        
            baseList.Add(new RotateAction(moveDir)); 
        }

        if (targetNode != null && targetNode.IsWalkable())
        {
            if (enemy.GridSensor.ScanForEnemy(moveDir, _attackRange))
            {
                baseList.Add(new MoveAction(targetNode));
                baseList.Add(new AttackAction(targetNode));
                return; 
            }
        
            baseList.Add(new MoveAction(targetNode));
        
            Node nodeAfterNext = targetNode.GetNodeInDirection(moveDir);

            if (nodeAfterNext == null || !nodeAfterNext.IsWalkable())
            {
                Direction reverseDir = GridMathUtility.TurnAround(moveDir);
                baseList.Add(new RotateAction(reverseDir, 0.1f));
            }
        }
    }
}