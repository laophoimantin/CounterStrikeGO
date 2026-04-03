using System.Collections;
using DG.Tweening;

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
        Tween attackTween = enemy.UnitCombat.GetAttackTween(_targetNode);
        yield return attackTween.WaitForCompletion();
    }
}