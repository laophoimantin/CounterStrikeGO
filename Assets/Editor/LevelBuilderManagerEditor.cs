using Grid;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelBuilderManager))]
public class LevelBuilderManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var builder = (LevelBuilderManager)target;
        var nodeManager = builder.NodeManager;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Map Generation Tools", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green; 
        if (GUILayout.Button("Generate New Map", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog("Generate Map", 
                "This will delete the existing map. Are you sure?", "Yes", "Cancel"))
            {
                builder.GenerateNodeMap();
                builder.RebuildNodeGrid(); 
                builder.AssignNodeNeighbors();
                
                EditorUtility.SetDirty(nodeManager);
            }
        }
        GUI.backgroundColor = Color.white; 

        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Re-Link Neighbors"))
        {
            builder.RebuildNodeGrid();
            builder.AssignNodeNeighbors();
            Debug.Log("Neighbors Relinked!");
        }

        if (GUILayout.Button("Rebuild Dictionary"))
        {
            builder.RebuildNodeGrid();
            Debug.Log("Node Grid Dictionary Rebuilt!");
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear Map"))
        {
            if (EditorUtility.DisplayDialog("Delete Map", 
                "Are you sure you want to delete all nodes?", "Yes", "Cancel"))
            {
                builder.DeleteMap();
                EditorUtility.SetDirty(nodeManager);
            }
        }
        GUI.backgroundColor = Color.white;
    }
}