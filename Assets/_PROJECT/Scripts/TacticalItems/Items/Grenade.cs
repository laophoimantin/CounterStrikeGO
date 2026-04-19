using DG.Tweening;

/// <summary>
/// Kill every enemies on one node
/// </summary>
public class Grenade : UtilityController
{
    protected override Tween GetOnLandedSequence(Node targetNode, Team team)
    {
        return CombatResolver.ResolveAttackOnNode(targetNode, team, false);
    }
}