using System.Collections;
using Grid;
using UnityEngine;

namespace Pawn.EnemyBehaviors
{
    public class MoveAction : BaseEnemyAction
    {
        private Node _targetNode;
        private float _duration;
        private float _delay;
        public MoveAction(Node target, float duration, float delay = 0.0f)
        {
            _targetNode = target;
            _duration = duration;
            _delay = delay;
        }

        public override IEnumerator Execute(EnemyController enemy)
        {
            yield return new WaitForSeconds(_delay);
            yield return enemy.Move(_targetNode, _duration); 
        }
    }
}