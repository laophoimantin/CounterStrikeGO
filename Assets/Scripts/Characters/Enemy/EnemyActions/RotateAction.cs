using System.Collections;
using UnityEngine;

namespace Characters.Enemy.EnemyActions
{
    public class RotateAction : BaseEnemyAction
    {
        private Quaternion _targetRotation;
        private float _duration;
        private float _delay;

        public RotateAction(Quaternion target, float duration, float delay = 0.0f)
        {
            _targetRotation = target;
            _duration = duration;
            _delay = delay;
        }

        public override IEnumerator Execute(EnemyController enemy)
        {
            yield return new WaitForSeconds(_delay);
            yield return enemy.Rotate(_targetRotation, _duration);
        }
    }
}