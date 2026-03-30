using UnityEngine;

[CreateAssetMenu(menuName = "Objective/Finish Under Turns")]
public class FinishUnderStepsObjective : BaseObjective
{
    [Space(10)]
    [SerializeField] private int _maxSteps;

    public override bool IsComplete(LevelResult result)
    {
        return result.GetData<int>(ContextKey.StepCount, 0) <= _maxSteps;
    }
}