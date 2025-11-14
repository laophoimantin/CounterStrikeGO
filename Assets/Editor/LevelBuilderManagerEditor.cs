using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelBuilderManager))]
public class LevelBuilderManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (LevelBuilderManager)target;

        if (GUILayout.Button("Save Level"))
        {
            Debug.Log("Saving Level...");
            script.SaveLevel();
        }

        if (GUILayout.Button("Generate Level"))
        {
            script.GenerateLevel();
        }
        
    }
}
