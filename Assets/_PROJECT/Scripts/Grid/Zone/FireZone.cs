using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.TurnSystem;
using Pawn;

namespace Grid
{
    public class FireZone : NodeZone
    {
        public override bool IsWalkable() => false; 
        public override IEnumerator OnUnitEnter()
        {
            var enemyList = _hostNode.GetUnitsByType<EnemyController>();
            return null;
        }
    }
}