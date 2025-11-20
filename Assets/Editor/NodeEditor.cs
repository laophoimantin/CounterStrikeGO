using System.Collections;
using System.Collections.Generic;
using Characters.Enemy;
using UnityEngine;
using UnityEditor;
using Grid;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (Node)target;

            EditorGUILayout.Space();
        
        if (GUILayout.Button("Remove Northside Node"))
        {
            script.RemoveNeighbour(Direction.North);
            EditorUtility.SetDirty(script);
        }
        if (GUILayout.Button("Remove Southside Node"))
        {
            script.RemoveNeighbour(Direction.South);
            EditorUtility.SetDirty(script);
        }
        if (GUILayout.Button("Remove Eastside Node"))
        {
            script.RemoveNeighbour(Direction.East);
            EditorUtility.SetDirty(script);
        }
        if (GUILayout.Button("Remove Westside Node"))
        {
            script.RemoveNeighbour(Direction.West);
            EditorUtility.SetDirty(script);
        }

        
    }
}
