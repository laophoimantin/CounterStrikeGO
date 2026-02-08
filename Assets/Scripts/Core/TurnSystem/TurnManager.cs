using UnityEngine;
using Core.Patterns;
using Core.Events;

namespace Core.TurnSystem
{
    public class TurnManager : Singleton<TurnManager>
    {
        [Header("Timing")] public float GlobalActionDuration = 1f;

        private TurnType _currentTurn;
        private bool _actionPhaseActive = false;
        private bool _lock = false;

        public TurnType CurrentTurn => _currentTurn;


        void OnEnable()
        {
            this.Subscribe<OnGameEndedEvent>(Lock);
            
            // Player Turn Events
            this.Subscribe<OnPlayerActionStartedEvent>(HandlePlayerStarted);
            this.Subscribe<OnPlayerActionFinishedEvent>(HandlePlayerFinished);

            //Enemy Turn Events
            this.Subscribe<OnEnemyActionStartedEvent>(HandleEnemyStarted);
            this.Subscribe<OnEnemyActionFinishedEvent>(HandleEnemyFinished);
        }

        void OnDisable()
        {
            this.Unsubscribe<OnGameEndedEvent>(Lock);
            
            // Player Turn Events
            this.Unsubscribe<OnPlayerActionStartedEvent>(HandlePlayerStarted);
            this.Unsubscribe<OnPlayerActionFinishedEvent>(HandlePlayerFinished);

            //Enemy Turn Events
            this.Unsubscribe<OnEnemyActionStartedEvent>(HandleEnemyStarted);
            this.Unsubscribe<OnEnemyActionFinishedEvent>(HandleEnemyFinished);

        }
        
        void Start()
        {
            SetTurn(TurnType.PlayerPlanning);
        }

        private void Lock(OnGameEndedEvent eventData)
        {
            _lock = true;
        }

        public void StartActionPhase()
        {
            if (_actionPhaseActive) return;
            _actionPhaseActive = true;
            
            if (_currentTurn == TurnType.PlayerPlanning)
                SetTurn(TurnType.PlayerAction);
            else if (_currentTurn == TurnType.EnemyPlanning)
                SetTurn(TurnType.EnemyAction);
        }
        
        public void EndActionPhase()
        {
            if (!_actionPhaseActive) return;
            _actionPhaseActive = false;

            switch (_currentTurn)
            {
                case TurnType.PlayerAction:
                    SetTurn(TurnType.EnemyPlanning);
                    break;

                case TurnType.EnemyAction:
                    SetTurn(TurnType.PlayerPlanning);
                    break;
            }
        }

        private void SetTurn(TurnType next)
        {
            if (_lock) return;
            _currentTurn = next;
            this.SendEvent(new OnTurnChangedEvent { NewTurn = next });
        }
        
        
        
        // Player Turn Events
        private void HandlePlayerStarted(OnPlayerActionStartedEvent eventData)
        {
            StartActionPhase();
        }

        private void HandlePlayerFinished(OnPlayerActionFinishedEvent eventData)
        {
            EndActionPhase();
        }

        //Enemy Turn Events
        private void HandleEnemyStarted(OnEnemyActionStartedEvent eventData)
        {
            StartActionPhase();
        }

        private void HandleEnemyFinished(OnEnemyActionFinishedEvent eventData)
        {
            EndActionPhase();
        }
    }
}