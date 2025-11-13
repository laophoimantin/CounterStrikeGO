using System.Collections;
using UnityEngine;
using Core.TurnSystem;
using Grid;

namespace Characters.Enemy.EnemyBehaviors
{
    public abstract class BaseEnemyBehavior: MonoBehaviour
    {
        [Range(0f, 10f)]
        [SerializeField] protected float _actionDurationModifier = 0f;
        protected float _actionDuration;

        protected void Start()
        {
            _actionDuration = TurnManager.Instance.ActionDuration;
        }
        public abstract IEnumerator Execute(EnemyController enemy);
    }
}