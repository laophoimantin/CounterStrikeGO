using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "CurrentLevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private string _levelId; // level_1, level_2, etc
    
    [SerializeField] private BaseObjective _mainObjective;
    [SerializeField] private List<BaseObjective> _optionalObjectives;
    
    // Public Fields
    public string LevelId => _levelId;
    public BaseObjective MainObjective => _mainObjective;
    public List<BaseObjective> OptionalObjectives => _optionalObjectives;
}

