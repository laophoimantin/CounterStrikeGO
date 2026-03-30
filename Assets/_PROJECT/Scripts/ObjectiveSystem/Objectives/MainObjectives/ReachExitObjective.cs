using UnityEngine;

[CreateAssetMenu(menuName = "Objective/ReachExitObjective", order = 0)]
public class ReachExitObjective : BaseObjective
{
    public override bool IsComplete(LevelResult result)
    {
        return true;
    }
}