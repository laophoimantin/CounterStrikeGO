using UnityEngine;

[CreateAssetMenu(menuName = "Objective/Kill None")]
public class EliminateNoneObjective : BaseObjective
{
    public override bool IsComplete(LevelResult result)
    {
        return !EnemyManager.Instance.HasKilledEnemy();
    }
}