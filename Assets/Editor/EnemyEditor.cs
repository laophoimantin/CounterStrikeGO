using Characters.Enemy;
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

            if (GUILayout.Button("Assign Current Node"))
            {
                script.SetOrMoveNode();
                EditorUtility.SetDirty(script);
            }
            
            if (GUILayout.Button("Unassign Current Node"))
            {
                script.UnAssignNode();
                EditorUtility.SetDirty(script);
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("North"))
            {
                script.SetDirection(Direction.North);
                EditorUtility.SetDirty(script);
            }
            if (GUILayout.Button("South"))
            {
                script.SetDirection(Direction.South);
                EditorUtility.SetDirty(script);
            }
            if (GUILayout.Button("East"))
            {
                script.SetDirection(Direction.East);
                EditorUtility.SetDirty(script);
            }
            if (GUILayout.Button("West"))
            {
                script.SetDirection(Direction.West);
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
