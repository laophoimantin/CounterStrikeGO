using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AttackAction : BaseEnemyAction
{
    private float _delay;
    private Node _targetNode;

    public AttackAction(Node targetNode)
    {
        _targetNode = targetNode;
    }

    public override IEnumerator Execute(EnemyController enemy)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(enemy.EnemyVisual.Wobble(true));
        seq.Join(enemy.UnitCombat.GetAttackTween(_targetNode));
        //Tween attackTween = enemy.UnitCombat.GetAttackTween(_targetNode);
        yield return seq.WaitForCompletion();
    }
}