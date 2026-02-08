using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn.EnemyBehaviors
{
    public class StaticBehavior : BaseEnemyBehavior
    {
        [Range(0.1f, 2f)] [SerializeField] private float _actionDuration = 1.0f;

        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();
            float duration = TurnManager.Instance.GlobalActionDuration * _actionDuration;

            if (enemy.ScanForPlayerInFront(1))
            {
                Node targetNode = enemy.GetNodeInFront();
                plan.Add(new MoveAction(targetNode, duration));
            }
            else
            {
                plan.Add(new WaitAction(0.01f));
            }
            return plan;
        }
    }
}