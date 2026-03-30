using System.Collections.Generic;
using UnityEngine;

public class ObjectivesController : MonoBehaviour
{
    private RuntimeObjective _mainObjective;
    private List<RuntimeObjective> _optionalObjectives = new();
    private string _currentLevelId;

    [SerializeField] private ObjectivesPanel _objectivesPanel;

    public void Initialize(LevelData currentLevelData)
    {
        Cleanup();
        _currentLevelId = currentLevelData.LevelId;

        InitializeObjectives(currentLevelData);
        InitializePanel();
    }


    private void Cleanup()
    {
        _mainObjective = null;
        _optionalObjectives.Clear();
    }
    // =======================================================================================

    private void InitializeObjectives(LevelData levelData)
    {
        bool isMainDone = SaveManager.Instance.IsObjectiveComplete(_currentLevelId, levelData.MainObjective.Id);
        _mainObjective = new RuntimeObjective(levelData.MainObjective, isMainDone);

        foreach (var obj in levelData.OptionalObjectives)
        {
            bool isOptDone = SaveManager.Instance.IsObjectiveComplete(_currentLevelId, obj.Id);
            _optionalObjectives.Add(new RuntimeObjective(obj, isOptDone));
        }
    }

    private void InitializePanel()
    {
        _objectivesPanel.Initialize(_mainObjective, _optionalObjectives);
    }

    public bool IsMainComplete()
    {
        return _mainObjective.IsCompleteNow;
    }

    public void EvaluateAll(LevelResult result)
    {
        _mainObjective.UpdateCompletedState(result);
        foreach (var obj in _optionalObjectives)
            obj.UpdateCompletedState(result);
    }

    public void SaveObjectiveStatus()
    {
        _mainObjective.TrySave(_currentLevelId);
        foreach (var obj in _optionalObjectives)
            obj.TrySave(_currentLevelId);

        SaveManager.Instance.SaveGame();
    }
}