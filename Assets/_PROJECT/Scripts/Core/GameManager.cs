using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private LevelData _currentLevelData;
    private LevelData _nextLevelData;

    [Header("References")]
    [SerializeField] private BoardInspectCamera _camScript;
    [SerializeField] private LevelSpawner _levelSpawner;
    [SerializeField] private ObjectivesController _objectivesController;

    private LevelResult _result;

    public bool IsGameOver { get; private set; }

    [Header("Testing")]
    [SerializeField] private bool _test;
    [SerializeField] private LevelData _testLevelData;
    [SerializeField] private GameObject _testMap;
    
    public event Action OnPlayerPickedUp;

    void Start()
    {
        InitLevel();
    }

    private void InitLevel()
    {
        if (_test)
        {
            IsGameOver = false;
            
            _result = new LevelResult();
            
            SessionData.SetCurrentLevelData(_testLevelData);
            _currentLevelData = SessionData.CurrentLevelData;
            _nextLevelData = SessionData.NextLevelDataToLoad;
            
            _camScript.ApplySettings(_testLevelData.CameraSetup);
            _objectivesController.Initialize(_testLevelData);
            return;
        }

        IsGameOver = false;
        
        _result = new LevelResult();
        _currentLevelData = SessionData.CurrentLevelData;
        _nextLevelData = SessionData.NextLevelDataToLoad;
        
        _levelSpawner.GenerateMap(_currentLevelData);
        _camScript.ApplySettings(_currentLevelData.CameraSetup);
        _objectivesController.Initialize(_currentLevelData);
    }

    void OnEnable()
    {
        this.Subscribe<OnPlayerDeadEvent>(LoseGame);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnPlayerDeadEvent>(LoseGame);
    }

    // =============================================================================================================

    public void OnPlayerPickedUpObjective()
    {
        _result.SetData<bool>(ContextKey.HasObjectiveItem, true);
        OnPlayerPickedUp?.Invoke();
    }

    public void EvaluateWin()
    {
        _result.SetData(ContextKey.StepCount, TurnManager.Instance.StepCount);
        _objectivesController.EvaluateAll(_result);
        if (!_objectivesController.IsMainComplete())
            return;

        WinGame();
    }

    private void WinGame()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        _objectivesController.SaveObjectiveStatus();
        if (_nextLevelData != null)
            SaveManager.Instance.SetLevelUnlocked(_nextLevelData.LevelId);

        SaveManager.Instance.SaveGame();

        this.SendEvent(new OnGameEndedEvent());
    }


    private void LoseGame(OnPlayerDeadEvent eventData)
    {
        if (IsGameOver) return;
        IsGameOver = true;

        SceneController.Instance.ReloadCurrentScene();
    }

    public void RequestNextLevel()
    {
        if (_nextLevelData == null)
        {
            SceneController.Instance.LoadMainMenu();
            return;
        }

        SessionData.SetCurrentLevelData(_nextLevelData);
        SceneController.Instance.LoadGameplayScene();
    }
}