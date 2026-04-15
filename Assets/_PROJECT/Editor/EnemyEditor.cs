using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyController))]
public class EnemyEditor : Editor
{
    EnemyController  script;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

         script = (EnemyController)target;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Design Tools", EditorStyles.boldLabel);

        // Node Management 
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Snap to Nearest Node", GUILayout.Height(50)))
        {
            Undo.RecordObject(script.transform, "Snap Enemy"); // Save Transform for Undo
            Undo.RecordObject(script, "Snap Enemy State");     // Save Script variables
            script.SetOrMoveNode();
        }
        // if (GUILayout.Button("Unassign Node"))
        // {
        //     Undo.RecordObject(script, "Unassign Node");
        //     script.UnAssignNode();
        // }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Direction Control (Compass Layout) 
        EditorGUILayout.LabelField("Rotate & Move", EditorStyles.boldLabel);

        Color defaultColor = GUI.backgroundColor;

        EditorGUILayout.Space(10);
        GUILayout.BeginVertical("box");
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        DrawButtonGroup("Face N", "Move N", Direction.North, isVertical: true, faceFirst: true);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        DrawButtonGroup("Face W", "Move W", Direction.West, isVertical: false, faceFirst: true);

        GUILayout.Space(40); 

        DrawButtonGroup("Face E", "Move E", Direction.East, isVertical: false, faceFirst: false);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10); 

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        DrawButtonGroup("Face S", "Move S", Direction.South, isVertical: true, faceFirst: false);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.EndVertical(); 
        EditorGUILayout.Space(10);

        GUI.backgroundColor = defaultColor;
    }

    void DrawButtonGroup(string faceText, string moveText, Direction dir, bool isVertical, bool faceFirst)
    {
        if (isVertical) GUILayout.BeginVertical();
        else GUILayout.BeginHorizontal();

        if (faceFirst)
        {
            DrawFaceBtn(faceText, dir);
            DrawMoveBtn(moveText, dir);
        }
        else
        {
            DrawMoveBtn(moveText, dir);
            DrawFaceBtn(faceText, dir);
        }

        if (isVertical) GUILayout.EndVertical();
        else GUILayout.EndHorizontal();
    }

    void DrawFaceBtn(string text, Direction dir)
    {
        GUI.backgroundColor = Color.blue;
        if (GUILayout.Button(text, GUILayout.Width(60), GUILayout.Height(25))) 
            ApplyDir(script, dir);
    }

    void DrawMoveBtn(string text, Direction dir)
    {
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button(text, GUILayout.Width(60), GUILayout.Height(25))) 
            ApplyMove(script, dir);
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