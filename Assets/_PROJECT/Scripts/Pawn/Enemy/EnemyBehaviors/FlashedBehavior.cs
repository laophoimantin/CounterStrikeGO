using Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Flashed", menuName = "Behav/Flashed", order = 11)]
    public class FlashedBehavior : BaseEnemyBehavior
    {
        public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
        {
            var plan = new List<BaseEnemyAction>();
            if (enemy.HasEndFlashed())
            {
                return plan;
            }

            plan.Add(new WaitAction(0.01f));
            return plan;
        }
    }
}
