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
            var enemyList = _hostNode.GetUnitsByType<EnemyController>().ToList();
            return Attack(enemyList);

        }

        private IEnumerator Attack(List<EnemyController> enemies)
        {
            int pending = enemies.Count;

            foreach (var enemy in enemies)
                enemy.Terminate(() => pending--);

            while (pending > 0)
                yield return null;
        }
    }
}