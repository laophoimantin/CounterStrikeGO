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
        [SerializeField] private float _delayTime = 0.3f;

        private readonly List<EnemyController> _activeEnemiesList = new();
        public bool AreAllEnemiesDefeated() => _activeEnemiesList.Count <= 0;

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
            StartCoroutine(BeginEnemyAction(snapshot));
        }


        private IEnumerator BeginEnemyAction(List<EnemyController> snapshot)
        {
            yield return new WaitForSeconds(_delayTime);
            this.SendEvent(new OnEnemyActionStartedEvent());

            if (snapshot.Count == 0)
            {
                yield return StartCoroutine(EndEnemyActionPhase());
                yield break;
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