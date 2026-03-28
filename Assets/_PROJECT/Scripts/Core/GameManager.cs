using Core.Events;
using Core.Patterns;
using Core.TurnSystem;
using UnityEngine;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        private LevelData _currentLevelData;
        private LevelData _nextLevelData;
        
        [Header("References")]
        [SerializeField] private ObjectivesController _objectivesController;

        private LevelResult _result;
        bool _isGameOver = false;
        
        void Start()
        {
            InitLevel();
        }
        
        private void InitLevel()
        {
            _isGameOver = false;
            _result = new LevelResult();
            _currentLevelData = SessionData.CurrentLevelData;
            _nextLevelData = SessionData.NextLevelDataToLoad;
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
            if (_isGameOver) return;

            _isGameOver = true;
            _objectivesController.SaveAll();
            if (_nextLevelData != null)
                SaveManager.Instance.SetLevelUnlocked(_nextLevelData.LevelId);
            
            _objectivesController.SaveAll();
            this.SendEvent(new OnGameEndedEvent());
        }


        private void LoseGame(OnPlayerDeadEvent eventData)
        {
            if (_isGameOver) return;
            _isGameOver = true;
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
}