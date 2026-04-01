using DG.Tweening;
using UnityEngine;

public abstract class PawnUnit : GridOccupant
{
    [SerializeField] protected float _actionDuration = 1;
    public float ActionDuration => _actionDuration;
    
    protected bool _isDead = false;
    public bool IsDead => _isDead;
    public override bool OccupiesSpace => !_isDead;
    
    public abstract Tween Die();
    
    [Header("Combat Identity")]
    [SerializeField] protected Team _team;
    public Team Team => _team; 
    public bool IsEnemyOf(PawnUnit otherUnit)
    {
        if (otherUnit == this || otherUnit.IsDead) return false;

        if (_team == Team.Neutral || otherUnit.Team == Team.Neutral) return false;

        return _team != otherUnit.Team;
    }
}
public enum Team
{
    Neutral,    
    PlayerTeam,   
    EnemyTeam 
}