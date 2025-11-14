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

    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    [SerializeField] private float _cellSize;

    public void GenerateLevel()
    {
        Debug.Log("Neee");
        m_NodeManager.GenerateMap(_mapWidth, _mapHeight, _cellSize);
    }



    public void SaveLevel()
    {
        // do save level logic

        // must have to save ScriptableObject changes
        //EditorUtility.SetDirty(mySO);
        Debug.Log("Save Successfully");
    }

    public void LoadLevel()
    {

    }
}
