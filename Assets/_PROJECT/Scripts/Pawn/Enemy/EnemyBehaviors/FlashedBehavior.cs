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
        protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
        {
            if (enemy.HasEndFlashed())
            {
                return;
            }
            baseList.Add(new WaitAction(0.01f));
        }
    }
}