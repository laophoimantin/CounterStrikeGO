using Pawn;
using Grid;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyController))]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (EnemyController)target;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Design Tools", EditorStyles.boldLabel);

        // Node Management 
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Snap to Nearest Node"))
        {
            Undo.RecordObject(script.transform, "Snap Enemy"); // Save Transform for Undo
            Undo.RecordObject(script, "Snap Enemy State");     // Save Script variables
            script.SetOrMoveNode();
        }
        if (GUILayout.Button("Unassign Node"))
        {
            Undo.RecordObject(script, "Unassign Node");
            script.UnAssignNode();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Direction Control (Compass Layout) 
        EditorGUILayout.LabelField("Rotate & Move", EditorStyles.boldLabel);

        // Top Row (North)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Face N", GUILayout.Width(60))) 
            ApplyDir(script, Direction.North);
        if (GUILayout.Button("Move N", GUILayout.Width(60))) 
            ApplyMove(script, Direction.North);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Middle Row (West / East)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Face W", GUILayout.Width(60))) 
            ApplyDir(script, Direction.West);
        if (GUILayout.Button("Move W", GUILayout.Width(60))) 
            ApplyMove(script, Direction.West);
        
        GUILayout.Space(20); // =============================================================
        
        if (GUILayout.Button("Face E", GUILayout.Width(60))) 
            ApplyDir(script, Direction.East);
        if (GUILayout.Button("Move E", GUILayout.Width(60))) 
            ApplyMove(script, Direction.East);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Bottom Row (South)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Face S", GUILayout.Width(60))) 
            ApplyDir(script, Direction.South);
        if (GUILayout.Button("Move S", GUILayout.Width(60))) 
            ApplyMove(script, Direction.South);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void ApplyDir(EnemyController script, Direction dir)
    {
        Undo.RecordObject(script.transform, "Rotate Enemy");
        Undo.RecordObject(script, "Rotate Enemy");
        script.SetDirection(dir);
        EditorUtility.SetDirty(script);
    }

    private void ApplyMove(EnemyController script, Direction dir)
    {
        Undo.RecordObject(script.transform, "Move Enemy");
        Undo.RecordObject(script, "Move Enemy");
        script.SetOrMoveNode(dir);
        EditorUtility.SetDirty(script);
    }
}