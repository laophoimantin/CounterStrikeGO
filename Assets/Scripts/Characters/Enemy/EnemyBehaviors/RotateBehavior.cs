using System.Collections;
using Core.TurnSystem;
using UnityEngine;

namespace Characters.Enemy.EnemyBehaviors
{
    public class RotateBehavior : BaseEnemyBehavior
    {
        #region Private Fields

        [SerializeField] private Transform _target;

        #endregion

        public override IEnumerator Execute(EnemyController enemy)
        {
            TurnManager.Instance.StartActionPhase();
            Quaternion startRot = _target.rotation;
            Quaternion endRot = startRot * Quaternion.Euler(0f, 180f, 0f);
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * (_actionDuration + _actionDurationModifier);
                _target.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }
        }
    }
}