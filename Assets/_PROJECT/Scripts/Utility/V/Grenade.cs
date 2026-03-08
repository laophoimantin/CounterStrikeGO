using DG.Tweening;
using Grid;
using Pawn;

public class Grenade : UtilityController
{
    protected override Sequence GetOnLandedSequence(Node targetNode)
    {
        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;

        float turn = 0;
        foreach (var enemy in targetNode.GetUnitsByType<EnemyController>())
        {
            turn += 0.3f;
            var death = enemy.Terminate();
            if (death == null) continue;
            seq.Insert(turn, death);
            hasReaction = true;
        }

        return hasReaction ? seq : null;
    }
}