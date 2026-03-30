using DG.Tweening;

public class Grenade : UtilityController
{
    protected override Tween GetOnLandedSequence(Node targetNode)
    {
        return EnemyCombatResolver.ResolveAttackOnNode(targetNode, false);
    }
}