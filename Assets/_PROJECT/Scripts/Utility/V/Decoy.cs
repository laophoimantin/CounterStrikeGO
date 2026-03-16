using Grid;
using Pawn;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Decoy : UtilityController
{
    protected override Tween GetOnLandedSequence(Node targetNode)
    {
        var nodes = NodeManager.Instance.GetNodesInRange(targetNode, 1, true);

        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;

        foreach (var node in nodes)
        {
            foreach (var enemy in node.GetUnitsByType<EnemyController>())
            {
                if (enemy.IsFlashed) continue;

                var reaction = enemy.HearNoise(targetNode);

                if (reaction == null) continue;

                seq.Insert(0, reaction);
                hasReaction = true;
            }
        }

        return hasReaction ? seq : null;
    }
}