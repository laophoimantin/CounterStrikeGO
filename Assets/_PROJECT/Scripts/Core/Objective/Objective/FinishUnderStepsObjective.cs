using Core.TurnSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Finish Under Turns")]
public class FinishUnderStepsObjective : BaseObjective
{
    [SerializeField] private int _maxSteps;
    public override bool IsComplete(LevelContext context)
    {
        return context.GetData<int>(ContextKey.StepCount, 0) <= _maxSteps;
    }
}