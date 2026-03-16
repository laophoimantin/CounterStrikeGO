using System.Collections.Generic;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Pawn
{
    [CreateAssetMenu(fileName = "Stand", menuName = "Behav/Stand", order = 1)]
    public class StaticBehavior : BaseEnemyBehavior
    {
        protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
        {
            baseList.Add(new WaitAction(0.01f));
        }
    }
}