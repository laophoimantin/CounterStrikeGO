using System.Collections.Generic;

/// <summary>
/// A dynamic blackboard for storing end of level achievements and statistics
/// </summary>
public class LevelResult
{
    private Dictionary<ContextKey, object> _data = new();

    public void SetData<T>(ContextKey key, T value)
    {
        _data[key] = value;
    }

    public T GetData<T>(ContextKey key, T defaultValue = default)
    {
        if (_data.TryGetValue(key, out object rawValue))
        {
            if (rawValue is T typedValue)
            {
                return typedValue;
            }
        }

        return defaultValue;
    }
}

public enum ContextKey
{
    StepCount,
    HasObjectiveItem
}