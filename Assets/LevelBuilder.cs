using System;
using System.Collections;
using System.Collections.Generic;
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

public enum NodeType
{
    Walkale,
    Water,
    None,
    Tree
}