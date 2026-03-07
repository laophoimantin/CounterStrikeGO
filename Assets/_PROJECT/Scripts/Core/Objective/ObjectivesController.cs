using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectivesController : MonoBehaviour
{
    private RuntimeObjective _mainObjective;
    private List<RuntimeObjective> _optionalObjectives = new();
    private string _currentLevelId;
    
    [SerializeField] private ObjectivesPanel _objectivesPanel;

    private LevelContext _context;

    public void Initialize(LevelData currentLevelData, LevelContext context)
    {
        _currentLevelId = currentLevelData.LevelId;
        _context = context;

        
        bool isMainDone = SaveManager.Instance.IsObjectiveComplete(_currentLevelId, currentLevelData.MainObjective.Id);
        _mainObjective = new RuntimeObjective(currentLevelData.MainObjective, isMainDone);
        _objectivesPanel.SpawnObjective(_mainObjective);
        
       foreach (var obj in currentLevelData.OptionalObjectives)
        {
            bool isOptDone = SaveManager.Instance.IsObjectiveComplete(_currentLevelId, obj.Id);
            var optObj = new RuntimeObjective(obj, isOptDone);
            _optionalObjectives.Add(optObj);
            _objectivesPanel.SpawnObjective(optObj);
        }
        _objectivesPanel.Initialize();
    }

    public bool IsMainComplete()
    {
        _mainObjective.UpdateCompletedState(_context);
        return _mainObjective.IsCompleteNow;
    }

    public void UpdateOptionalCompletedState()
    {
        foreach (var obj in _optionalObjectives)
        {
            obj.UpdateCompletedState(_context);
        }
    }
}
