using Core.TurnSystem;
using Grid;
using Pawn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Grenade : UtilityController
{
    protected override Sequence GetOnLandedSequence(Node targetNode)
    {
        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;

        foreach (var enemy in targetNode.GetUnitsByType<EnemyController>())
        {
            var death = enemy.Terminate();

            if (death == null) continue;

            seq.Insert(0, death);
            hasReaction = true;
        }

        return hasReaction ? seq : null;
    }
}