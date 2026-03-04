public class RuntimeObjective
{
    public BaseObjective Blueprint { get; private set; }
    public bool IsCompleteBefore { get; private set; }
    public bool IsCompleteNow { get; private set; }

    public RuntimeObjective(BaseObjective blueprint)
    {
        Blueprint = blueprint;
        
        //IsCompleteBefore = DataLoader.Instance.GetObjectiveData(blueprint.Id).IsCompleted;
        IsCompleteNow = false;
    }

    public bool CheckProgress(LevelContext context)
    {
        if (!IsCompleteNow && Blueprint.IsComplete(context))
        {
            IsCompleteNow = true;
        }
        return IsCompleteNow;
    }
}