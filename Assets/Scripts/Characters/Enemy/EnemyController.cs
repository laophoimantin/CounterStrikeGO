using System;
using System.Collections;
using Characters.Enemy.EnemyBehaviors;
using Core.TurnSystem;
using Core.Events;
using Interfaces;
using EventType = Core.Events.EventType;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private BaseEnemyBehavior _currentBehavior;

        #endregion

        #region Public Fields

        public BaseEnemyBehavior CurrentBehavior => _currentBehavior;

        #endregion

        void OnEnable()
        {
            EnemyTurnCoordinator.Instance.RegisterEnemy(this);
        }

        void OnDisable()
        {
            if (EnemyTurnCoordinator.Instance != null)
                EnemyTurnCoordinator.Instance.UnregisterEnemy(this);
        }

        void Start()
        {
        }

        public void StartAction(object param)
        {
            StartCoroutine(ExecuteBehavior());
        }

        private IEnumerator ExecuteBehavior()
        {
            if (_currentBehavior != null)
                yield return _currentBehavior.Execute(this);
            EnemyTurnCoordinator.Instance.OnEnemyFinished(this);
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}