using Grid;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class LevelBuilderManager : MonoBehaviour
{
    [SerializeField] private LevelBuilder m_LevelBuilder;
    [SerializeField] private NodeManager m_NodeManager;
    public NodeManager NodeManager => m_NodeManager;

    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    [SerializeField] private float _cellSize;

    public void GenerateNodeMap()
    {
        m_NodeManager.GenerateMap(_mapWidth, _mapHeight, _cellSize);
        
    }

    public void DeleteMap()
    {
        m_NodeManager.DeleteAllNodes();
    }

    public void AssignNodeNeighbors()
    {
        m_NodeManager.AssignNodeNeighbour();
    }

    public void RebuildNodeGrid()
    {
        m_NodeManager.RebuildNodeGrid();
    }


    public void SaveLevel()
    {

        // must have to save ScriptableObject changes
        //EditorUtility.SetDirty(mySO);
        Debug.Log("Save Successfully");
    }

    public void LoadLevel()
    {

    }
}
