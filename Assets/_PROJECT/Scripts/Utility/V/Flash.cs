using Grid;
using Pawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : UtilityController
{
    [SerializeField] private int _flashAmount = 2;
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
            yield return Flashing(affectedEnemies, targetNode);
        }
    }
    private IEnumerator Flashing(List<EnemyController> enemies, Node targetNode)
    {
        int pending = enemies.Count;

        foreach (var enemy in enemies)
            enemy.GetFlashed(_flashAmount, () => pending--);

        while (pending > 0)
            yield return null;
    }
}