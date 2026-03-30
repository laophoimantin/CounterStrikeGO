using UnityEngine;

[CreateAssetMenu(menuName = "Objective/ReachExitWithItemObjective", order = 1)]
public class ReachExitWithItemObjective : BaseObjective
{
    public override bool IsComplete(LevelResult result)
    {
        return result.GetData<bool>(ContextKey.HasObjectiveItem, false);
    }
}