using Core.TurnSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Objectives/Finish Under Turns")]
public class FinishUnderStepsObjective : BaseObjective
{
    [SerializeField] private int _maxSteps;


    public override bool IsComplete(LevelContext context)
    {
        return context.StepCount <= _maxSteps;
    }
}