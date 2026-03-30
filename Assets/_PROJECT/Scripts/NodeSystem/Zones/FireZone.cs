using DG.Tweening;

public class FireZone : BaseZone
{
    public override bool IsWalkable() => false;

    protected override Tween OnZoneCreated()
    {
        return BurnOccupants();
    }

    private Tween BurnOccupants()
    {
        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;
      
        foreach (GridOccupant occupant in _currentNode.GetAllOccupants())
        {
            IBurnable meat = occupant.GetComponent<IBurnable>();
            
            if (meat != null)
            {
                Tween roastAnim = meat.Burn();

                if (roastAnim != null)
                {
                    seq.Insert(0, roastAnim);
                    hasReaction = true;
                }
            }
        }
        
        // foreach (var enemy in _currentNode.GetUnitsByType<EnemyController>())
        // {
        //     var death = enemy.ReactToFire();
        //     if (death == null) continue;
        //     seq.Insert(0, death);
        //     hasReaction = true;
        // }

        return hasReaction ? seq : null;
    }
}