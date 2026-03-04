using UnityEngine;

public abstract class BaseObjective : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _description;
    
    public string Id => _id;
    public Sprite Icon => _icon;
    public string Description => _description;

    
    public abstract bool IsComplete(LevelContext context);
}

