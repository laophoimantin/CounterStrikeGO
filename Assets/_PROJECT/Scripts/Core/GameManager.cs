using Core.Events;
using Core.Patterns;
using Core.TurnSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Data")]
        [SerializeField] private LevelData _currentLevelData;
        
        [Header("References")]
        [SerializeField] private ObjectivesController _objectivesController;

        private LevelContext _context;
        bool _isGameOver = false;

        protected override void Awake()
        {
            base.Awake();

            _context = new LevelContext();
            
            // DataLoader.Instance.SetLevelSaveDataBaseOnIdIsUnlocked(true, _currentLevelData.Id);
            
            if (_objectivesController != null)
                _objectivesController.Initialize(_currentLevelData, _context);
        }

        void OnEnable()
        {
            this.Subscribe<OnPlayerDeadEvent>(LoseGame);
            this.Subscribe<OnPlayerSteppedEvent>(OnPlayerStep);
        }

        void OnDisable()
        {
            this.Unsubscribe<OnPlayerDeadEvent>(LoseGame);
            this.Unsubscribe<OnPlayerSteppedEvent>(OnPlayerStep);
        }

        // =============================================================================================================

        public void OnPlayerPickedUpObjective()
        {
            _context.SetData<bool>(ContextKey.HasObjectiveItem, true);
        }

        private void OnPlayerStep(OnPlayerSteppedEvent e)
        {
            int currentSteps = _context.GetData<int>(ContextKey.StepCount, 0); 
    
            _context.SetData(ContextKey.StepCount, currentSteps + 1);
        }

        public void EvaluateWin()
        {
            if (!_objectivesController.IsMainComplete())
                return;

            _objectivesController.UpdateOptionalCompletedState();
            WinGame();
        }

        private void WinGame()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            this.SendEvent(new OnGameEndedEvent { });
            Debug.Log("LEVEL COMPLETE!");
        }

        private void LoseGame(OnPlayerDeadEvent eventData)
        {
            if (_isGameOver) return;
            _isGameOver = true;
            SceneController.Instance.ReloadCurrentScene();
            Debug.Log("GAME OVER");
        }
    }
}