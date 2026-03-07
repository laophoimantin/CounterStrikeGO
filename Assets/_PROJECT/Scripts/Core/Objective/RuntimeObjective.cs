using UnityEngine.ProBuilder.MeshOperations;

public class RuntimeObjective
{
    public BaseObjective Blueprint { get; }
    public bool IsCompleteBefore { get; private set; }
    public bool IsCompleteNow { get; private set; }

    public RuntimeObjective(BaseObjective blueprint, bool isCompletedBefore)
    {
        Blueprint = blueprint;
        IsCompleteBefore = isCompletedBefore;
        IsCompleteNow = isCompletedBefore;
    }

    public void UpdateCompletedState(LevelContext context)
    {
        if (!IsCompleteBefore)
        {
            IsCompleteNow = Blueprint.IsComplete(context);
        }
    }
}