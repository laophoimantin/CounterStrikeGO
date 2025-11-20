using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Grid;

[CustomEditor(typeof(LevelBuilderManager))]
public class LevelBuilderManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (LevelBuilderManager)target;

        NodeManager nodeManager = script.NodeManager;


        if (GUILayout.Button("Assign Node"))
        {
            script.AssignNodeNeighbors();
            EditorUtility.SetDirty(nodeManager);
        }

        if (GUILayout.Button("Generate Map"))
        {
            script.GenerateNodeMap();
            EditorUtility.SetDirty(nodeManager);
        }

        if (GUILayout.Button("Delete Map"))
        {
            script.DeleteMap();
            EditorUtility.SetDirty(nodeManager);
        } 
        if (GUILayout.Button("Rebuild Node"))
        {
            script.RebuildNodeGrid();
            EditorUtility.SetDirty(nodeManager);
        }
        
        

        // if (GUILayout.Button("Save Level"))
        // {
        //     Debug.Log("Saving Level...");
        //     script.SaveLevel();
        // }
    }
}