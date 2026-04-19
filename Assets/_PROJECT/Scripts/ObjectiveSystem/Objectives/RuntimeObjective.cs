/// <summary>
/// Runtime version of objective
/// </summary>

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

    public void UpdateCompletedState(LevelResult result)
    {
        if (!IsCompleteBefore)
        {
            IsCompleteNow = Blueprint.IsComplete(result);
        }
    }

    public void TrySave(string levelId)
    {
        if (!IsCompleteBefore && IsCompleteNow)
        {
            SaveManager.Instance.SetObjectiveComplete(levelId, Blueprint.Id);
        }
    }
}