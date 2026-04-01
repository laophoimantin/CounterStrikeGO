using UnityEngine;
using System.Collections.Generic;

public class GridSensor : MonoBehaviour
{
    private PawnUnit _controller;

    private void Awake()
    {
        _controller = GetComponent<PawnUnit>();
    }

    public bool ScanForEnemy(Direction facingDir, int range)
    {
        Node nodeToScan = _controller.CurrentNode.GetNodeInDirection(facingDir);

        while (nodeToScan != null && range > 0)
        {
            if (!nodeToScan.IsWalkable()) return false;
            if (nodeToScan.IsHideable()) return false;

            foreach (GridOccupant occupant in nodeToScan.GetAllOccupants())
            {
                if (occupant is PawnUnit victim && _controller.IsEnemyOf(victim))
                {
                    return true;
                }
            }

            range--;
            nodeToScan = nodeToScan.GetNodeInDirection(facingDir);
        }
        return false;
    }

    public Node FindEscapeNode(Direction currentFacing)
    {
        for (int i = 0; i < 4; i++)
        {
            Direction dir = GridMathUtility.GetDirectionByStep(currentFacing, i);
            Node node = _controller.CurrentNode.GetNodeInDirection(dir);
            
            if (node != null && node.IsWalkable())
                return node;
        }
        return null;
    }
}