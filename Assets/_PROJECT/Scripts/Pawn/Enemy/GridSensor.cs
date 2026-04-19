using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Linear line-of-sight and identifying valid escape routes.
/// </summary>
public class GridSensor : MonoBehaviour
{
    private PawnUnit _controller;

    private void Awake()
    {
        _controller = GetComponent<PawnUnit>();
    }

    public bool ScanForEnemy(Direction facingDir, int range)
    {
        Node currentNode = _controller.CurrentNode;

        for (int i = 0; i < range; i++)
        {
            Node nodeToScan = currentNode.GetNodeInDirection(facingDir);

            if (nodeToScan == null)
            {
                return false;
            }
            
            if (!nodeToScan.IsWalkable() || nodeToScan.IsHideable())
                return false;
            
            foreach (GridOccupant occupant in nodeToScan.GetAllOccupants())
            {
                if (occupant is PawnUnit victim && _controller.IsEnemyOf(victim))
                {
                    return true;
                }
            }
            currentNode = nodeToScan;
        }

        return false;
        
        // Node nodeToScan = _controller.CurrentNode.GetNodeInDirection(facingDir);
        //
        // while (nodeToScan != null && range > 0)
        // {
        //     if (!nodeToScan.IsWalkable()) return false;
        //     if (nodeToScan.IsHideable()) return false;
        //
        //     foreach (GridOccupant occupant in nodeToScan.GetAllOccupants())
        //     {
        //         if (occupant is PawnUnit victim && _controller.IsEnemyOf(victim))
        //         {
        //             return true;
        //         }
        //     }
        //
        //     range--;
        //     nodeToScan = nodeToScan.GetNodeInDirection(facingDir);
        // }
        //
        // return false;
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

    //public bool ScanForEnemy2(Direction facingDir, int range)
    //{
    //    int checkX = _controller.CurrentNode.XValue;
    //    int checkY = _controller.CurrentNode.YValue;


    //    Vector2Int step = GridMathUtility.DirectionToVector(facingDir);
    //    for (int i = 0; i < range; i++)
    //    {
    //        checkX += step.x;
    //        checkY += step.y;

    //        if (!NodeManager.Instance.TryGetNode(checkX, checkY, out Node nodeToScan))
    //        {
    //            return false;
    //        }

    //        if (!nodeToScan.IsWalkable() || nodeToScan.IsHideable())
    //            return false;

    //        foreach (GridOccupant occupant in nodeToScan.GetAllOccupants())
    //        {
    //            if (occupant is PawnUnit victim && _controller.IsEnemyOf(victim))
    //            {
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}

}