using System;
using System.Collections.Generic;
using UnityEngine;

public class AstarPathfinder
{
    public class PathNode
    {
        public Node Node;
        public PathNode Parent;

        public int G;
        public int H;
        public int F => G + H;

        public PathNode(Node node)
        {
            Node = node;
        }
    }

    public static List<Node> FindPath(Node startNode, Node goalNode)
    {
        if (startNode == null || goalNode == null)
            throw new ArgumentNullException();

        var openList = new List<PathNode>();
        var closeList = new HashSet<Node>();

        var startPathNode = new PathNode(startNode)
        {
            G = 0,
            H = Heuristic(startNode, goalNode)
        };

        openList.Add(startPathNode);

        while (openList.Count > 0)
        {
            PathNode currentPathNode = openList[0];
            foreach (var pathNode in openList)
            {
                if (pathNode.F < currentPathNode.F || (pathNode.F == currentPathNode.F && pathNode.H < currentPathNode.H))
                {
                    currentPathNode = pathNode;
                }
            }

            if (currentPathNode.Node == goalNode)
            {
                return ReconstructPath(currentPathNode);
            }

            openList.Remove(currentPathNode);
            closeList.Add(currentPathNode.Node);

            foreach (var neighbour in currentPathNode.Node.GetNeighbour())
            {
                if (neighbour == null) continue;
                if (!neighbour.IsWalkable()) continue;
                if (closeList.Contains(neighbour)) continue;

                int tentativeG = currentPathNode.G + 1;
                PathNode existing = openList.Find(n => n.Node == neighbour);
                if (existing == null)
                {
                    var newPathNode = new PathNode(neighbour)
                    {
                        Parent = currentPathNode,
                        G = tentativeG,
                        H = Heuristic(neighbour, goalNode)
                    };
                    openList.Add(newPathNode);
                }
                else if (tentativeG < existing.G)
                {
                    existing.Parent = currentPathNode;
                    existing.G = tentativeG;
                }
            }
        }

        return null; // No path, too bad, so sad
    }


    public static int Heuristic(Node start, Node target)
    {
        Vector2Int startCoor = start.Get2DCoordinate();
        Vector2Int targetCoor = target.Get2DCoordinate();
        return Mathf.Abs(targetCoor.x - startCoor.x) + Mathf.Abs(targetCoor.y - startCoor.y);
    }

    public static List<Node> ReconstructPath(PathNode endNode)
    {
        var path = new List<Node>();
        PathNode currentPath = endNode;

        while (currentPath != null)
        {
            path.Add(currentPath.Node);
            currentPath = currentPath.Parent;
        }

        path.Reverse();


        return path;
    }
}