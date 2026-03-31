using System;
using UnityEngine;

public class GridMathUtility
{
    private static readonly Direction[] Dirs = 
    {
        Direction.North, Direction.East, Direction.South, Direction.West
    };

    // 1. TÍNH TOÁN GÓC XOAY THEO HƯỚNG ======================================
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

    // 2. TÍNH TOÁN PHƯƠNG HƯỚNG TỪ NODE NÀY SANG NODE KIA ===================
    public static Direction GetDirectionFromTargetNode(Node from, Node to, Direction fallback = Direction.None)
    {
        if (from == null || to == null) return fallback;

        if (from.NorthNode == to) return Direction.North;
        if (from.SouthNode == to) return Direction.South;
        if (from.EastNode == to) return Direction.East;
        if (from.WestNode == to) return Direction.West;

        return fallback; 
    }

    // 3. TÍNH TOÁN XOAY BƯỚC (QUAY ĐẦU, TRÁI, PHẢI) =========================
    public static Direction GetDirectionByStep(Direction currentDir, int step)
    {
        int index = Array.IndexOf(Dirs, currentDir);
        if (index < 0) return currentDir; // Đề phòng truyền bậy bạ Direction.None

        // Toán học xịn: Xử lý mượt mà số âm (xoay trái) mà đéo bao giờ văng Exception OutOfBound
        int newIndex = (index + (step % 4) + 4) % 4; 
        return Dirs[newIndex];
    }

    // Các hàm ăn liền (Syntactic Sugar) cho code đọc sướng như thơ:
    public static Direction TurnAround(Direction current) => GetDirectionByStep(current, 2);
    public static Direction TurnClockwise(Direction current) => GetDirectionByStep(current, 1);
    public static Direction TurnCounterClockwise(Direction current) => GetDirectionByStep(current, -1);
}
