using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RotateAction : BaseEnemyAction
{
    private Direction _targetDir;
    private float _delay;

    public RotateAction(Direction targetDir, float delay = 0.0f)
    {
        _targetDir = targetDir;
        _delay = delay;
    }

    public override IEnumerator Execute(EnemyController enemy)
    {
        yield return new WaitForSeconds(_delay);
        yield return enemy.Rotate(_targetDir).WaitForCompletion();
    }
}