using System;
using Grid;
using UnityEngine;

[Serializable]
public class LevelBuilder
{
    public int size;
    public ClassNode[] nodes;
}

[Serializable]
public class ClassNode
{
    public Vector2Int position;
    public NodeType nodeType;
}
