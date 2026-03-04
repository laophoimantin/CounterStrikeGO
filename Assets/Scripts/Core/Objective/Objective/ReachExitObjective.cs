using UnityEngine;

[CreateAssetMenu(fileName = "ReachExitObjective", menuName = "Objective/ReachExitObjective", order = 1)]
public class ReachExitObjective : BaseObjective 
{
    public override bool IsComplete(LevelContext context)
    {
        return true;
    }
}
