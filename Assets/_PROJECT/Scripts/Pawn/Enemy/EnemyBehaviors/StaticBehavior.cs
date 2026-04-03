using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stand", menuName = "Behav/Stand", order = 1)]
public class StaticBehavior : StandardEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        baseList.Add(new WaitAction(0.01f));
    }
}