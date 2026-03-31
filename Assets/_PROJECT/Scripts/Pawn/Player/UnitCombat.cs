using DG.Tweening;
using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    private PawnUnit _controller;

    private void Awake()
    {
        _controller = GetComponent<PawnUnit>();
    }
    
    public bool CanAttack(Node node)
    {
        return CombatResolver.CanAttack(node, _controller.Team);
    }
    public Tween GetAttackTween(Node targetNode)
    {
        return CombatResolver.ExecuteAttack(targetNode, _controller.Team);
    }

    public void AppendAttackSequence(Node targetNode, Sequence seq)
    {
        var combatTween = GetAttackTween(targetNode);
        
        if (combatTween != null)
        {
            seq.Append(combatTween);
        }
    }
}