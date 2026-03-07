using System.Collections.Generic;

public class LevelContext
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

            UnityEngine.Debug.LogError($"[LevelContext] Ép kiểu ngu rồi! Key '{key}' đang chứa kiểu {rawValue.GetType()}, nhưng mày lại đòi lấy kiểu {typeof(T)}.");
        }

        return defaultValue;
    }
}

public enum ContextKey
{
    StepCount,
    HasObjectiveItem
}