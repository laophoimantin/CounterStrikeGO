using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "Behav/Sniper", order = 2)]
public class SniperBehavior : BaseEnemyBehavior
{
    public override int ExecutionPriority => 1;

    public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
    {
        var plan = new List<BaseEnemyAction>();
        ScanForPlayerIgnoreLink(enemy, plan);

        return plan;
    }

    private void ScanForPlayerIgnoreLink(EnemyController sniper, List<BaseEnemyAction> plan)
    {
        int checkX = sniper.CurrentNode.XValue;
        int checkY = sniper.CurrentNode.YValue;
        Node lastNode = sniper.CurrentNode;

        Vector2Int step = GridMathUtility.DirectionToVector(sniper.CurrentFacingDirection);
        while (true)
        {
            checkX += step.x;
            checkY += step.y;


            if (!NodeManager.Instance.TryGetNode(checkX, checkY, out Node nextNode))
            {
                break;
            }

            lastNode = nextNode;

            if (nextNode.IsObstacle || nextNode.IsHideable())
            {
                break;
            }

            // teammates block vison
            if (HasOccupantOfRelation(nextNode, sniper, isEnemy: false))
            {
                break;
            }

            // player found
            if (HasOccupantOfRelation(nextNode, sniper, isEnemy: true))
            {
                plan.Add(new AttackAction(nextNode));
                break;
            }

        }

        this.SendEvent(new OnSniperTargetDetectedEvent
        {
            Sniper = sniper,
            TargetNode = lastNode
        });
    }

    private bool HasOccupantOfRelation(Node node, EnemyController self, bool isEnemy)
    {
        foreach (GridOccupant occupant in node.GetAllOccupants())
        {
            if (occupant is PawnUnit unit && self.IsEnemyOf(unit) == isEnemy)
                return true;
        }

        return false;
    }
}