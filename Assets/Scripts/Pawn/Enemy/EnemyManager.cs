using System;
using System.Collections;
using System.Collections.Generic;
using Pawn;
using Core.Events;
using Core.Patterns;
using UnityEngine;
using Grid;

namespace Core.TurnSystem
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        [SerializeField] private float _delayTime = 0.3f;

        private readonly List<EnemyController> _activeEnemiesList = new();
        public int GetActiveEnemyCount() => _activeEnemiesList.Count;
        
        private List<EnemyController> _pendingEnemies;
        private int _finishedCount = 0;

        void OnEnable()
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);
        }

        void OnDisable()
        {
            this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChange);
            
        }

        // Enemy registration =======================================================

        public void RegisterEnemy(EnemyController enemy)
        {
            if (!_activeEnemiesList.Contains(enemy))
            {
                _activeEnemiesList.Add(enemy);
                enemy.OnDestroyed += UnregisterEnemy;
            }
        }

        private void UnregisterEnemy(EnemyController enemy)
        {
            if (_activeEnemiesList.Contains(enemy))
            {
                _activeEnemiesList.Remove(enemy);
        
                GameManager.Instance.CheckEliminationWinCondition();
            }
        }


        // Turn System =========================================================================
        private void HandleTurnChange(OnTurnChangedEvent eventData)
        {
            if (eventData.NewTurn != TurnType.EnemyPlanning)
                return;
            InitializeEnemyTurnState();
            StartCoroutine(BeginEnemyAction());
        }

        private void InitializeEnemyTurnState()
        {
            _finishedCount = 0;
            _pendingEnemies = new List<EnemyController>(_activeEnemiesList);
        }

        private IEnumerator BeginEnemyAction()
        {
            yield return new WaitForSeconds(_delayTime);
            this.SendEvent(new OnEnemyActionStartedEvent());

            _pendingEnemies.RemoveAll(e => e == null);

            if (_pendingEnemies.Count == 0)
            {
                _pendingEnemies.Clear();
                StartCoroutine(EndEnemyActionPhase());
                yield break;
            }

            foreach (var enemy in _pendingEnemies)
            {
                if (enemy != null)
                {
                    enemy.StartAction();
                }
                else
                {
                    OnEnemyFinished(null);
                }
            }
        }

        public void OnEnemyFinished(EnemyController enemy)
        {
            _finishedCount++;
            if (_finishedCount >= _pendingEnemies.Count)
            {
                _pendingEnemies.Clear();
                StartCoroutine(EndEnemyActionPhase());
            }
        }

        private IEnumerator EndEnemyActionPhase()
        {
            yield return new WaitForSeconds(_delayTime);
            this.SendEvent(new OnEnemyActionFinishedEvent());
        }
    }
}