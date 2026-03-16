using System.Linq;
using DG.Tweening;
using Grid;
using Pawn;

public static class CombatResolver
{
    public static Tween ResolveAttackOnNode(Node node, bool checkHideable = true)
    {
        if (node == null)
            return null;

        if (checkHideable && node.IsHideable())
            return null;
        
        var enemies = node.GetUnitsByType<EnemyController>().ToList();
        if (enemies.Count == 0)
            return null;

        Sequence seq = DOTween.Sequence();

        foreach (var enemy in enemies)
        {
            Tween death = enemy.Die();

            if (death == null) 
                continue;

            death.OnComplete(() => enemy.FinishDeath());

            seq.Join(death);
        }

        return seq;
    }
}