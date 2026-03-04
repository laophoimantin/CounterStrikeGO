using System.Collections;
using UnityEngine;

namespace Pawn
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
            if (_duration > 0f)
            {
                yield return new WaitForSeconds(_duration);
            }
            else
            {
                yield return null;
            }
        }
    }
}