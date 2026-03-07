using UnityEngine;

[CreateAssetMenu(fileName = "ReachExitWithItemObjective", menuName = "Objective/ReachExitWithItemObjective", order = 10)]
public class ReachExitWithItemObjective : BaseObjective
{
    public override bool IsComplete(LevelContext context)
    {
        return context.GetData<bool>(ContextKey.HasObjectiveItem, false);
    }
}
