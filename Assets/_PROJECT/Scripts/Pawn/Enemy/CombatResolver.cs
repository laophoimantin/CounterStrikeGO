using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

/// <summary>
/// utility for evaluating and executing combat interactions on grid nodes
/// </summary>
public static class CombatResolver
{
    public static bool CanAttack(Node node, Team attackerTeam, bool checkHideable = true)
    {
        if (node == null || (checkHideable && node.IsHideable())) return false;

        foreach (GridOccupant occupant in node.GetAllOccupants())
        {
            if (occupant is PawnUnit unit && unit.Team != attackerTeam && !unit.IsDead)
            {
                return true; 
            }
        }
        return false;
    }
    
    public static Tween ExecuteAttack(Node node, Team attackerTeam, bool checkHideable = true)
    {
        var victims = GetVictims(node, attackerTeam, checkHideable);

        if (victims.Count == 0) 
            return null;

        Sequence seq = DOTween.Sequence();

        foreach (var victim in victims)
        {
            var death = victim.Die();
            if (death != null)
                seq.Join(death);
        }

        return seq;
    }
    
    private static List<PawnUnit> GetVictims(Node node, Team attackerTeam, bool checkHideable = true)
    {
        List<PawnUnit> victims = new();

        if (node == null || (checkHideable && node.IsHideable()))
            return victims;

        foreach (GridOccupant occupant in node.GetAllOccupants())
        {
            if (occupant is PawnUnit unit && unit.Team != attackerTeam && !unit.IsDead)
            {
                victims.Add(unit);
            }
        }

        return victims;
    }
    
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