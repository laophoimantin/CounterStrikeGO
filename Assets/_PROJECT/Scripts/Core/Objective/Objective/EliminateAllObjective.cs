using Core.TurnSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Objective/Eliminate All")]
public class EliminateAllObjective : BaseObjective
{

    public override bool IsComplete(LevelResult result)
    {
        return EnemyManager.Instance.AreAllEnemiesDefeated();
    }
}