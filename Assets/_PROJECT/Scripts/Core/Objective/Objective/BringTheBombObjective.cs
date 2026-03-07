using Core;
using Core.TurnSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Bring The Bomb")]
public class BringTheBombObjective : BaseObjective
{
    public override bool IsComplete(LevelContext context)
    {
        return context.GetData<bool>(ContextKey.HasObjectiveItem, false);
    }
}