using Core.TurnSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Eliminate All")]
public class EliminateAllObjective : BaseObjective
{

    public override bool IsComplete(LevelContext context)
    {
        return EnemyManager.Instance.AreAllEnemiesDefeated();
    }
}