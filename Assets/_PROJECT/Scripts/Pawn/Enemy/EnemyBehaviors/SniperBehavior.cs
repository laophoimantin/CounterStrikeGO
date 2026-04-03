using System.Collections.Generic;
using UnityEngine;

public class SniperBehavior : BaseEnemyBehavior
{
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

        Vector2Int step = GridMathUtility.DirectionToVector(sniper.CurrentFacingDirection);
        while (true)
        {
            checkX += step.x;
            checkY += step.y;

            if (!NodeManager.Instance.TryGetNode(checkX, checkY, out Node nextNode))
            {
                break;
            }

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