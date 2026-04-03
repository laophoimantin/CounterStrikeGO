using System;
using UnityEngine;

public static class GridMathUtility
{
    private static readonly Direction[] Dirs = 
    {
        Direction.North, Direction.East, Direction.South, Direction.West
    };

    public static Quaternion GetRotation(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Quaternion.Euler(0, 0, 0);
            case Direction.South: return Quaternion.Euler(0, 180, 0);
            case Direction.East: return Quaternion.Euler(0, 90, 0);
            case Direction.West: return Quaternion.Euler(0, -90, 0);
            default: return Quaternion.identity; 
        }
    }

    public static Direction GetDirectionFromTargetNode(Node from, Node to, Direction fallback = Direction.None)
    {
        if (from == null || to == null) return fallback;

        if (from.NorthNode == to) return Direction.North;
        if (from.SouthNode == to) return Direction.South;
        if (from.EastNode == to) return Direction.East;
        if (from.WestNode == to) return Direction.West;

        return fallback; 
    }

    public static Direction GetDirectionByStep(Direction currentDir, int step)
    {
        int index = Array.IndexOf(Dirs, currentDir);
        if (index < 0) return currentDir;

        int newIndex = (index + (step % 4) + 4) % 4; 
        return Dirs[newIndex];
    }

    // wrap
    
    public static Direction TurnAround(Direction current) => GetDirectionByStep(current, 2);
    public static Direction TurnClockwise(Direction current) => GetDirectionByStep(current, 1);
    public static Direction TurnCounterClockwise(Direction current) => GetDirectionByStep(current, -1);
    
    public static Vector2Int DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return new Vector2Int(0, 1);
            case Direction.South: return new Vector2Int(0, -1);
            case Direction.East:  return new Vector2Int(1, 0);
            case Direction.West:  return new Vector2Int(-1, 0);
            default:              return Vector2Int.zero; 
        }
    }
}
