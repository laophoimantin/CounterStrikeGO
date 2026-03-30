using UnityEngine;

[CreateAssetMenu(menuName = "Objective/Bring The Bomb")]
public class BringTheBombObjective : BaseObjective
{
    public override bool IsComplete(LevelResult result)
    {
        return result.GetData<bool>(ContextKey.HasObjectiveItem, false);
    }
}