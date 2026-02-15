using System;
using System.Collections;
using System.Collections.Generic;
using Pawn;
using Core.Events;
using Core.Patterns;
using UnityEngine;

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
                TurnManager.Instance.EndActionPhase();
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


        private int _pendingKills = 0;

        public void ResolveAttack(List<GridUnit> enemies, Action onComplete)
        {
            if (enemies.Count == 0)
            {
                onComplete?.Invoke();
                return;
            }

            _pendingKills = enemies.Count;

            foreach (var unit in enemies)
            {
                if (unit is EnemyController enemy)
                {
                    enemy.Die(() => OnEnemyDeathComplete(onComplete));
                }
                else
                {
                    OnEnemyDeathComplete(null);
                }
            }
        }

        private void OnEnemyDeathComplete(Action onComplete)
        {
            _pendingKills--;
            if (_pendingKills <= 0)
            {
                onComplete?.Invoke();
            }
        }
    }
}