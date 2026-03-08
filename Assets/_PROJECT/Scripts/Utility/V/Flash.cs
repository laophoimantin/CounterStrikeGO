using Grid;
using Pawn;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Flash : UtilityController
{
    [SerializeField] private int _flashAmount = 2;
 
    protected override Sequence GetOnLandedSequence(Node targetNode)
    {
        var nodes = NodeManager.Instance.GetNodesInRange(targetNode, 1, true);
        
        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;
        
        foreach (var node in nodes)
        {
            foreach (var enemy in node.GetUnitsByType<EnemyController>())
            {
                if (!enemy.OccupiesSpace) continue;

                var reaction = enemy.GetFlashed(_flashAmount);

                if (reaction == null) continue;

                seq.Insert(0, reaction);
                hasReaction = true;
            }
        }

        return hasReaction ? seq : null;
        
        //
        // var surroundingNodes = NodeManager.Instance.GetNodesInRange(targetNode, 1, true);
        // List<EnemyController> affectedEnemies = new();
        // foreach (var node in surroundingNodes)
        // {
        //     affectedEnemies.AddRange(node.GetUnitsByType<EnemyController>());
        // }
        //
        // if (affectedEnemies.Count == 0) return null;
        //
        // Sequence reactionSequence = DOTween.Sequence();
        //
        // foreach (var enemy in affectedEnemies)
        // {
        //     Sequence enemyReaction = enemy.GetFlashed(_flashAmount);
        //     if (enemyReaction != null)
        //     {
        //         reactionSequence.Insert(0, enemyReaction); 
        //     }
        // }
        //
        // return reactionSequence;
    }

}