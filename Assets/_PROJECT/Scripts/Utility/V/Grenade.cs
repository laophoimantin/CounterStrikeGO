using DG.Tweening;
using Grid;
using Pawn;

public class Grenade : UtilityController
{
    protected override Tween GetOnLandedSequence(Node targetNode)
    {
        return CombatResolver.ResolveAttackOnNode(targetNode, false);
    }
}