using Grid;
using Pawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : UtilityController
{
    protected override IEnumerator OnLanded(Node targetNode)
    {
        List<EnemyController> affectedEnemies = new();
        var surroundingNodes = NodeManager.Instance.GetNodesInRange(targetNode, 1, true);
        foreach (var node in surroundingNodes)
        {
            affectedEnemies.AddRange(node.GetUnitsByType<EnemyController>());
        }

        if (affectedEnemies.Count > 0)
        {
            Debug.Log("Decoy landed on " + targetNode.WorldPos + " and found " + affectedEnemies.Count + " enemies");
            yield return Lure(affectedEnemies, targetNode);
            Debug.Log("Decoy lured " + affectedEnemies.Count + " enemies0");
        }
    }
    private IEnumerator Lure(List<EnemyController> enemies, Node targetNode)
    {
        int pending = enemies.Count;

        foreach (var enemy in enemies)
            enemy.HearNoise(targetNode, () => pending--);

        while (pending > 0)
            yield return null;
    }
}