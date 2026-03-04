using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Stand", menuName = "Behav/Stand", order = 1)]
    public class StaticBehavior : BaseEnemyBehavior
    {
        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();
            if (enemy.ScanForPlayerInFront(1))
            {
                Node targetNode = enemy.GetNodeInFront();
                plan.Add(new MoveAction(targetNode, Duration));
            }
            else
            {
                plan.Add(new WaitAction(0.01f));
            }
            return plan;
        }
    }
}