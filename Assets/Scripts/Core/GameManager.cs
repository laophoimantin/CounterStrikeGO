using Characters.Player;
using Core.Events;
using Core.Patterns;
using Core.TurnSystem;
using Grid;
using UnityEngine;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Level Objectives")]
        [SerializeField] private bool _objectiveIsRequired = true;

        [Header("Game State")]
        private bool _playerHasObjective = false;
        private bool _isGameOver = false;

        public bool PlayerHasObjective => _playerHasObjective;

        
        
        
        
        
        void OnEnable()
        {
            // Player Turn Events
            this.Subscribe<OnPlayerActionStartedEvent>(HandlePlayerStarted);
            this.Subscribe<OnPlayerActionFinishedEvent>(HandlePlayerFinished);
            
            //Enemy Turn Events
            this.Subscribe<OnEnemyActionStartedEvent>(HandleEnemyStarted);
            this.Subscribe<OnEnemyActionFinishedEvent>(HandleEnemyFinished);
            
            //Game Over Event
            this.Subscribe<OnPlayerDeadEvent>(LoseGame);
        }

        void OnDisable()
        {
            // Player Turn Events
            this.Unsubscribe<OnPlayerActionStartedEvent>(HandlePlayerStarted);
            this.Unsubscribe<OnPlayerActionFinishedEvent>(HandlePlayerFinished);
            
            //Enemy Turn Events
            this.Unsubscribe<OnEnemyActionStartedEvent>(HandleEnemyStarted);
            this.Unsubscribe<OnEnemyActionFinishedEvent>(HandleEnemyFinished);
            
            //Game Over Event
            this.Unsubscribe<OnPlayerDeadEvent>(LoseGame);
        }

        
        
        
        // Event Handlers ==============================================================================================
        // Player Turn Events
        private void HandlePlayerStarted(OnPlayerActionStartedEvent eventData)
        {
            TurnManager.Instance.StartActionPhase();
        }        
        private void HandlePlayerFinished(OnPlayerActionFinishedEvent eventData)
        {
            TurnManager.Instance.EndActionPhase();
        }
        
        //Enemy Turn Events
        private void HandleEnemyStarted(OnEnemyActionStartedEvent eventData)
        {
            TurnManager.Instance.StartActionPhase();
        }

        private void HandleEnemyFinished(OnEnemyActionFinishedEvent eventData)
        {
            TurnManager.Instance.EndActionPhase();
        }
        // =============================================================================================================
        
        
        
        
        
        
        
        
        public void OnPlayerPickedUpObjective()
        {
            _playerHasObjective = true;
            Debug.Log("Player picked up the objective!");
        }

        public void CheckWinCondition()
        {
            if (_isGameOver) return;
            if (_objectiveIsRequired && !_playerHasObjective)
            {
                Debug.Log("You need the objective first!");
                return;
            }

            WinGame();
        }

        
        
        
        
        private void WinGame()
        {
            if (_isGameOver) return;
            
            _isGameOver = true;
            this.SendEvent(new OnGameEndedEvent{});
            Debug.Log("LEVEL COMPLETE!");
            Time.timeScale = 0; 
        }

        private void LoseGame(OnPlayerDeadEvent eventData)
        {
            if (_isGameOver) return;
            this.SendEvent(new OnGameEndedEvent{});
            _isGameOver = true;
            Debug.Log("GAME OVER");
            Time.timeScale = 0;
        }
    }
}