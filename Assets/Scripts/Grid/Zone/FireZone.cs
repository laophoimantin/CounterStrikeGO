using System;
using System.Collections.Generic;
using Core.TurnSystem;
using Pawn;

namespace Grid
{
    public class FireZone : NodeZone
    {
        public override bool IsWalkable() => false; 
        public override void OnUnitEnter(List<GridUnit> units, Action onComplete)
        {
            List<GridUnit> enemies = new List<GridUnit>();
            foreach (var unit in units)
            {
                if (unit is EnemyController)
                {
                    enemies.Add(unit);
                }
            }
            EnemyManager.Instance.ResolveAttack(enemies, onComplete);
        }
    }
}