using DG.Tweening;

public class FireZone : BaseZone
{
    public override bool IsWalkable() => false;

    protected override Tween OnZoneCreated()
    {
        return PushEnemiesAway();
    }

    private Tween PushEnemiesAway()
    {
        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;
        foreach (var enemy in _currentNode.GetUnitsByType<EnemyController>())
        {
            var death = enemy.ReactToFire();
            if (death == null) continue;
            seq.Insert(0, death);
            hasReaction = true;
        }

        return hasReaction ? seq : null;
    }
}