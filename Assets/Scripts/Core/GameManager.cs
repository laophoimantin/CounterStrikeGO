using Core.Events;
using Core.Patterns;
using Core.TurnSystem;
using UnityEngine;

namespace Core
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Win Rules (Check all that apply)")]
        [Tooltip("The Player wins by killing all enemies")]
        [SerializeField] private bool _enableEliminationWin = true;

        [Tooltip("The Player wins by reaching the Exit Node")]
        [SerializeField] private bool _enableExitWin = true;

        [Tooltip("The player must have the objective item")]
        [SerializeField] private bool _exitRequiresObjective = false;

        [Header("Game State")]
        private bool _playerHasObjective = false;
        private bool _isGameOver = false;

        public bool PlayerHasObjective => _playerHasObjective;

        void OnEnable()
        {
            //Game Over Event
            this.Subscribe<OnPlayerDeadEvent>(LoseGame);
        }

        void OnDisable()
        {
            //Game Over Event
            this.Unsubscribe<OnPlayerDeadEvent>(LoseGame);
        }

        // =============================================================================================================

        public void OnPlayerPickedUpObjective()
        {
            _playerHasObjective = true;
        }

        public void CheckEliminationWinCondition()
        {
            if (_isGameOver || !_enableEliminationWin) return;

            if (EnemyManager.Instance.GetActiveEnemyCount() <= 0)
            {
                Debug.Log("Win Condition Met: All Hostiles Eliminated.");
                WinGame();
            }
        }
        
        public void CheckExitWinCondition()
        {
            if (_isGameOver || !_enableExitWin) return;

            if (_exitRequiresObjective && !_playerHasObjective)
            {
                Debug.Log("Need the Objective Item before escaping");
                return;
            }

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
            this.SendEvent(new OnGameEndedEvent { });
            _isGameOver = true;
            Debug.Log("GAME OVER");
        }
    }
}