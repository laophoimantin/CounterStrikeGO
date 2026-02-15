using System;
using Core.TurnSystem;
using Grid;
using UnityEngine;

public class Grenade : UtilityController
{
    protected override void OnLanded(Node targetNode, Action onComplete)
    {
        if (targetNode.HasEnemy())
        {
            var enemies = targetNode.GetEnemies();
            EnemyManager.Instance.ResolveAttack(enemies, onComplete);
            Die();
        }
        else
        {
            Die(onComplete);
        }
    }
}