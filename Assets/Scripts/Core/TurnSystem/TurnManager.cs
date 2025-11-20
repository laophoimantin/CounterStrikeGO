using UnityEngine;
using Core.Patterns;
using Core.Events;
using EventType = Core.Events.EventType;

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
        }

        void OnDisable()
        {
            this.Unsubscribe<OnGameEndedEvent>(Lock);
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
    }
}