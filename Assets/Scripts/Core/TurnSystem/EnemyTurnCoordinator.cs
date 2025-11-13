using System;
using System.Collections.Generic;
using Characters.Enemy;
using Core.Events;
using Core.Patterns;
using EventType = Core.Events.EventType;

namespace Core.TurnSystem
{
    public class EnemyTurnCoordinator : Singleton<EnemyTurnCoordinator>
    {
        private readonly List<EnemyController> _activeEnemiesList = new();
        private List<EnemyController> _activeEnemiesListTmp;

        private int _finishedCount = 0;

        #region Event Handerlers

        private Action<object> _onTurnChangedCallback;

        #endregion

        void OnEnable()
        {
            this.Subscribe<OnTurnChangedEvent>(StartEnemyTurn);
            
        }

        void OnDisable()
        {
            if (NewEventDispatcher.Instance != null)
            {
                this.Unsubscribe<OnTurnChangedEvent>(StartEnemyTurn);
            }
        }


        public void RegisterEnemy(EnemyController enemy)
        {
            if (!_activeEnemiesList.Contains(enemy))
            {
                _activeEnemiesList.Add(enemy);
            }
        }

        public void UnregisterEnemy(EnemyController enemy)
        {
            _activeEnemiesList.Remove(enemy);
        }

        private void StartEnemyTurn(OnTurnChangedEvent eventData)
        {
            TurnType turnType = eventData.NewTurn;
            if (turnType != TurnType.EnemyPlanning) return;
            
            _finishedCount = 0;
            _activeEnemiesListTmp = new List<EnemyController>(_activeEnemiesList);
            foreach (var enemy in _activeEnemiesListTmp)
            {
                enemy.StartAction();
            }
            TurnManager.Instance.StartActionPhase();
        }

        public void OnEnemyFinished(EnemyController enemy)
        {
            _finishedCount++;
            if (_finishedCount >= _activeEnemiesListTmp.Count)
            {
                _activeEnemiesListTmp.Clear();
                TurnManager.Instance.EndTurn();
            }
        }
    }
}