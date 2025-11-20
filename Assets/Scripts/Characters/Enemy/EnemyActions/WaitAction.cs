using System.Collections;
using UnityEngine;

namespace Characters.Enemy.EnemyActions
{
    public class WaitAction : BaseEnemyAction
    {
        private float _duration;
        private float _delay;

        public WaitAction(float duration)
        {
            _duration = duration;
        }

        public override IEnumerator Execute(EnemyController enemy)
        {
            yield return new WaitForSeconds(_duration);
        }
    }
}