using System.Collections;
using Core.TurnSystem;
using Grid;
using Unity.VisualScripting;
using UnityEngine;

namespace Characters.Enemy.EnemyBehaviors
{
    public class RotateBehavior : BaseEnemyBehavior
    {
        #region Private Fields

        [SerializeField] private Transform _target;
        private Node _nodeToScan;         

        #endregion

        public override IEnumerator Execute(EnemyController enemy)
        {
            Node target = enemy.GetNodeInDirection(enemy.CurrentNode, enemy.FacingDirection);

            if (target != null && !target.IsObstacle)
            {
                enemy.UpdateNodeData(target);
                
                float duration = TurnManager.Instance.ActionDuration * _actionDurationModifier;
                yield return enemy.StartCoroutine(enemy.MoveOverTime(target, duration));
            }
            else
            {
                float duration = TurnManager.Instance.ActionDuration * _actionDurationModifier;
                Quaternion targetRot = enemy.GetRotationTurnAround();
                yield return enemy.StartCoroutine(enemy.RotateOverTime(targetRot, duration));
            }
        }
    }
}