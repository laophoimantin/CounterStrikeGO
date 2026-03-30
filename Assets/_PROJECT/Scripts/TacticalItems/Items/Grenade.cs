using DG.Tweening;

public class Grenade : UtilityController
{
    protected override Tween GetOnLandedSequence(Node targetNode, Team team)
    {
        return CombatResolver.ResolveAttackOnNode(targetNode, team, false);
    }
}