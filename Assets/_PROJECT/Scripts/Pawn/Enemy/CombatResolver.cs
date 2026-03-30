using System.Linq;
using DG.Tweening;

public static class CombatResolver
{
    public static Tween ResolveAttackOnNode(Node node,Team attackerTeam, bool checkHideable = true)
    {
        if (node == null || (checkHideable && node.IsHideable()))
            return null;

        Sequence seq = DOTween.Sequence();

        bool hasVictims = false;
        
        foreach (GridOccupant occupant in node.GetAllOccupants())
        {
            if (occupant is PawnUnit victim)
            {
                if (victim.Team != attackerTeam && !victim.IsDead) 
                {
                    Tween death = victim.Die(); 

                    if (death != null)
                    {
                        seq.Join(death);
                        hasVictims = true;
                    }
                }
            }
        }

        return hasVictims ? seq : null;
    }
}