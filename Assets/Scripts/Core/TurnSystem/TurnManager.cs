using UnityEngine;
using Core.Patterns;
using Core.Events;
using EventType = Core.Events.EventType;

namespace Core.TurnSystem
{
    public class TurnManager : Singleton<TurnManager>
    {
        #region Private Fields

        private TurnType _currentTurn = TurnType.PlayerPlanning;
        private TurnType _nextTurn;

        #endregion

        #region Public Fields

        public TurnType CurrentTurn => _currentTurn;
        public float ActionDuration = 4f;

        #endregion

        public void StartActionPhase()
        {
            switch (_currentTurn)
            {
                case TurnType.PlayerPlanning:
                    _currentTurn = TurnType.PlayerAction;
                    break;
                case TurnType.EnemyPlanning:
                    _currentTurn = TurnType.EnemyAction;
                    break;
            }

            this.FireEvent(EventType.OnTurnChanged, _currentTurn);
        }

        public void EndTurn()
        {
            switch (_currentTurn)
            {
                case TurnType.PlayerAction:
                    _currentTurn = TurnType.EnemyPlanning;
                    break;
                case TurnType.EnemyAction:
                    _currentTurn = TurnType.PlayerPlanning;
                    break;
                default:
                    return;
            }

            this.FireEvent(EventType.OnTurnChanged, _currentTurn);
        }
    }
}