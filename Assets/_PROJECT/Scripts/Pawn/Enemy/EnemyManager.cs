using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pawn;
using Core.Events;
using Core.Patterns;
using UnityEngine;
using Grid;

namespace Core.TurnSystem
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private readonly List<EnemyController> _activeEnemiesList = new();
        private bool _hasKilledEnemy;
        public bool AreAllEnemiesDefeated() => _activeEnemiesList.Count <= 0;
        public bool HasKilledEnemy() => _hasKilledEnemy;

        private int _pendingCount;
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
            _activeEnemiesList.Remove(enemy);
            if (!_hasKilledEnemy)
                _hasKilledEnemy = true;
        }

        // Turn System =========================================================================
        private void HandleTurnChange(OnTurnChangedEvent eventData)
        {
            if (eventData.NewTurn != TurnType.EnemyPlanning)
                return;
            //InitializeEnemyTurnState();
            List<EnemyController> snapshot = _activeEnemiesList
                .Where(enemy => enemy != null)
                .ToList();
            BeginEnemyAction(snapshot);
        }


        private void BeginEnemyAction(List<EnemyController> snapshot)
        {
            this.SendEvent(new OnEnemyActionStartedEvent());

            if (snapshot.Count == 0)
            {
                EndEnemyActionPhase();
                return;
            }

            _finishedCount = 0;
            _pendingCount = snapshot.Count;

            foreach (var enemy in snapshot)
            {
                enemy.StartAction();
            }
        }

        public void OnEnemyFinished(EnemyController enemy)
        {
            _finishedCount++;
            if (_finishedCount >= _pendingCount)
            {
                EndEnemyActionPhase();
            }
        }

        private void EndEnemyActionPhase()
        {
            this.SendEvent(new OnEnemyActionFinishedEvent());
        }
    }
}