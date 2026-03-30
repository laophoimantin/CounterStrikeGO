using System.Collections.Generic;
using UnityEngine;

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