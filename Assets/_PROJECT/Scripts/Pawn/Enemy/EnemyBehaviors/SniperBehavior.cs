using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "Behav/Sniper", order = 2)]
public class SniperBehavior : BaseEnemyBehavior
{
    public override int ExecutionPriority => 1;

    public override void OnStart(EnemyController enemy)
    {
        Node laserEndNode = GetLaserTargetNode(enemy);

        this.SendEvent(new OnSniperTargetDetectedEvent
        {
            Sniper = enemy,
            TargetNode = laserEndNode
        });
    }

    public override void OnEnter(EnemyController enemy)
    {
        Node laserEndNode = GetLaserTargetNode(enemy);

        this.SendEvent(new OnSniperTargetDetectedEvent
        {
            Sniper = enemy,
            TargetNode = laserEndNode
        });
    }

    public override void OnExit(EnemyController enemy)
    {
        this.SendEvent(new OnSniperTargetDetectedEvent
        {
            Sniper = enemy,
            TargetNode = null 
        });
    }

    public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
    {
        var plan = new List<BaseEnemyAction>();

        Node targetNode = GetLaserTargetNode(enemy);

        this.SendEvent(new OnSniperTargetDetectedEvent
        {
            Sniper = enemy,
            TargetNode = targetNode
        });

        if (HasOccupantOfRelation(targetNode, enemy, isEnemy: true) && !targetNode.IsHideable())
        {
            plan.Add(new AttackAction(targetNode));
        }

        return plan;
    }

    private Node GetLaserTargetNode(EnemyController sniper)
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
                break;

            lastNode = nextNode;

            if (nextNode.IsObstacle || nextNode.IsHideable()) 
                break;
            if (HasOccupantOfRelation(nextNode, sniper, isEnemy: false)) 
                break;
            if (HasOccupantOfRelation(nextNode, sniper, isEnemy: true)) 
                break;
        }

        return lastNode;
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