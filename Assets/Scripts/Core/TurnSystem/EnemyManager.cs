using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using Core.Events;
using Core.Patterns;
using UnityEditor.Rendering;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Core.TurnSystem
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private List<EnemyController> _activeEnemiesList = new();
        private List<EnemyController> _pendingEnemies;

        private int _finishedCount = 0;

        void OnEnable()
        {
            this.Subscribe<OnTurnChangedEvent>(HandleTurnChange);
        }

        void OnDisable()
        {
            if (NewEventDispatcher.Instance != null)
            {
                this.Unsubscribe<OnTurnChangedEvent>(HandleTurnChange);
            }
        }
        
        // Enemy registration =======================================================

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
        
        
        // Turn System =========================================================================
        // private void HandleTurnChange(OnTurnChangedEvent eventData)
        // {
        //     if (eventData.NewTurn != TurnType.EnemyPlanning)
        //         return;
        //
        //     StartEnemyTurn();
        // }
        //
        // private void StartEnemyTurn()
        // {
        //     TurnManager.Instance.StartActionPhase();
        //     InitializeEnemyTurnState();
        //
        //     if (!HasPendingEnemies())
        //     {
        //         _pendingEnemies.Clear();
        //         TurnManager.Instance.EndActionPhase();
        //         return;
        //     }
        //
        //     StartPendingEnemiesActions();
        // }
        
        private void HandleTurnChange(OnTurnChangedEvent eventData)
        {
            if (eventData.NewTurn != TurnType.EnemyPlanning)
                return;
            InitializeEnemyTurnState();
            StartCoroutine(BeginEnemyActionNextFrame());
        }

        private IEnumerator BeginEnemyActionNextFrame()
        {
            yield return null; 
            TurnManager.Instance.StartActionPhase();

            if (!HasPendingEnemies())
            {
                _pendingEnemies.Clear();
                TurnManager.Instance.EndActionPhase();
                yield break;
            }

            StartPendingEnemiesActions();
        }
        
        

        private void InitializeEnemyTurnState()
        {
            _finishedCount = 0;
            _pendingEnemies = new List<EnemyController>(_activeEnemiesList);
        }

        private bool HasPendingEnemies()
        {
            return _pendingEnemies is { Count: > 0 };
        }

        private void StartPendingEnemiesActions()
        {
            foreach (var enemy in _pendingEnemies)
            {
                enemy.StartAction();
            }
        }

        public void OnEnemyFinished(EnemyController enemy)
        {
            _finishedCount++;
            if (_finishedCount >= _pendingEnemies.Count)
            {
                _pendingEnemies.Clear();
                TurnManager.Instance.EndActionPhase();
            }
        }
        
        
        
        
        private int _pendingKills  = 0;
        private PlayerController _currentAttacker;
        
        public void ResolveAttack(List<EnemyController> enemies, PlayerController attacker)
        {
            if (enemies.Count == 0)
            {
                attacker.SendEvent(new OnPlayerActionFinishedEvent());
                return;
            }
            _pendingKills = enemies.Count;
            foreach (var enemy in enemies)
            {
                enemy.Die(OnEnemyDeathComplete);
            }
        }

        private void OnEnemyDeathComplete()
        {
            _pendingKills--;
            if (_pendingKills <= 0)
                _currentAttacker.SendEvent(new OnPlayerActionFinishedEvent());
        }
        
        
        
    }
}