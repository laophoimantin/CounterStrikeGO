using UnityEngine;

public abstract class BaseObjective : ScriptableObject
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;

    public string Id => name;
    public Sprite Icon => _icon;
    public string Description => _description;

    public abstract bool IsComplete(LevelResult result);
}