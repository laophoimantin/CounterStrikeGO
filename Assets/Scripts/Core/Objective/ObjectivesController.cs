using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectivesController : MonoBehaviour
{
    private RuntimeObjective _mainObjective;
    private List<RuntimeObjective> _optionalObjectives = new();
    
    [SerializeField] private ObjectivesPanel _objectivesPanel;

    private LevelContext _context;

    public void Initialize(LevelData currentLevelData, LevelContext context)
    {
        _context = context;

        _objectivesPanel.Initialize();
        
        _mainObjective = new RuntimeObjective(currentLevelData.MainObjective);
        _objectivesPanel.SpawnObjective(_mainObjective);
        
        foreach (var obj in currentLevelData.OptionalObjectives)
        {
            var runtimeObj = new RuntimeObjective(obj);
            _optionalObjectives.Add(runtimeObj);
            _objectivesPanel.SpawnObjective(runtimeObj);
        }
    }

    public bool IsMainComplete()
    {
        return _mainObjective.CheckProgress(_context);
    }

    public IEnumerable<RuntimeObjective> GetCompletedOptional()
    {
        return _optionalObjectives.Where(o => o.CheckProgress(_context));
    }
}
