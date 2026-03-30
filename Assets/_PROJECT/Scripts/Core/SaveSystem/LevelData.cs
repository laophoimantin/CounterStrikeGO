using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level X", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    [SerializeField] private GameObject _mapPrefab;

    [SerializeField] private LevelData _nextLevel;
    [SerializeField] private bool _isUnlockedByDefault;
    [SerializeField] private int _levelDisplayNumber;
    [SerializeField] private BaseObjective _mainObjective;
    [SerializeField] private List<BaseObjective> _optionalObjectives;

    // Public Fields

    public GameObject MapPrefab => _mapPrefab;
    public LevelData NextLevel => _nextLevel;
    public bool IsUnlockedByDefault => _isUnlockedByDefault;
    public int LevelDisplayNumber => _levelDisplayNumber;

    public string LevelId => name;
    public BaseObjective MainObjective => _mainObjective;
    public List<BaseObjective> OptionalObjectives => _optionalObjectives;
}