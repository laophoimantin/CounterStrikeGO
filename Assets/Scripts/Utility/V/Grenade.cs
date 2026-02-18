using Core.TurnSystem;
using Grid;
using Pawn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grenade : UtilityController
{
    protected override IEnumerator OnLanded(Node targetNode)
    {
        if (targetNode.HasUnitsOfType<EnemyController>())
        {
            var enemyList = targetNode.GetUnitsByType<EnemyController>().ToList();
            yield return Attack(enemyList);
        }
    }

    private IEnumerator Attack(List<EnemyController> enemies)
    {
        int pending = enemies.Count;

        foreach (var enemy in enemies)
            enemy.Terminate(() => pending--);

        while (pending > 0)
            yield return null;
    }
}