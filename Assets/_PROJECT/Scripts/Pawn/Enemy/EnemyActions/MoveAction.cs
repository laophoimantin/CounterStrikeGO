using System.Collections;
using DG.Tweening;
using Grid;
using UnityEngine;

namespace Pawn
{
    public class MoveAction : BaseEnemyAction
    {
        private Node _targetNode;
        private float _delay;
        public MoveAction(Node target, float delay = 0.0f)
        {
            _targetNode = target;
            _delay = delay;
        }

        public override IEnumerator Execute(EnemyController enemy)
        {
            yield return new WaitForSeconds(_delay);
            yield return enemy.Move(_targetNode).WaitForCompletion(); 
        }
    }
}