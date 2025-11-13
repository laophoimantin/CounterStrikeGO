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
        private OldNode _nodeToScan;         

        #endregion

        public override IEnumerator Execute(EnemyController enemy)
        {


            if (enemy.CheckForPlayer(1))
            {
                yield return enemy.StartCoroutine(enemy.Move(enemy.GetNodeInDirection(enemy.CurrentNode, enemy.CurrentFacingDirection), TurnManager.Instance.ActionDuration * _actionDurationModifier));
            }
            else
            {
                float duration = TurnManager.Instance.ActionDuration * _actionDurationModifier;
                Quaternion targetRot = enemy.GetRotationTurnAround();
                yield return enemy.StartCoroutine(enemy.Rotate(targetRot, duration));
            }

        }
    }
}