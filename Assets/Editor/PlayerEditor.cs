using Characters.Player;
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
        
        if (GUILayout.Button("Assign Current Node"))
        {
            script.SetOrMoveNode();
            EditorUtility.SetDirty(script);
        }
            
        EditorGUILayout.Space();

        if (GUILayout.Button("Move North"))
        {
            script.SetOrMoveNode(Direction.North);
            EditorUtility.SetDirty(script);
        }
        if (GUILayout.Button("Move South"))
        {
            script.SetOrMoveNode(Direction.South);
            EditorUtility.SetDirty(script);
        }
        if (GUILayout.Button("Move East"))
        {
            script.SetOrMoveNode(Direction.East);
            EditorUtility.SetDirty(script);
        }
        if (GUILayout.Button("Move West"))
        {
            script.SetOrMoveNode(Direction.West);
            EditorUtility.SetDirty(script);
        }

    }
}