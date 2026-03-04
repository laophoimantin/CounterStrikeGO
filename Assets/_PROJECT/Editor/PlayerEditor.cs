using Pawn;
using UnityEditor;
using UnityEngine;
using Grid;

[CustomEditor(typeof(PlayerController))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (PlayerController)target;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Design Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Snap to Nearest Node"))
        {
            Undo.RecordObject(script.transform, "Snap Player");
            Undo.RecordObject(script, "Snap Player");
            script.SetOrMoveNode();
        }
        
        EditorGUILayout.Space();

        // Compass Layout
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Move North", GUILayout.Width(100))) ApplyMove(script, Direction.North);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("West", GUILayout.Width(60))) ApplyMove(script, Direction.West);
        GUILayout.Space(20);
        if (GUILayout.Button("East", GUILayout.Width(60))) ApplyMove(script, Direction.East);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Move South", GUILayout.Width(100))) ApplyMove(script, Direction.South);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void ApplyMove(PlayerController script, Direction dir)
    {
        Undo.RecordObject(script.transform, "Move Player");
        Undo.RecordObject(script, "Move Player");
        script.SetOrMoveNode(dir);
    }
}