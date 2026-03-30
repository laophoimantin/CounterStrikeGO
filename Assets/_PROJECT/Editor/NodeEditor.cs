using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var node = (Node)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Disconnect Neighbors", EditorStyles.boldLabel);

        // Top Row (North)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("X North", GUILayout.Width(80))) 
            ApplyDisconnect(node, Direction.North);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Middle Row (West / East)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("X West", GUILayout.Width(80))) 
            ApplyDisconnect(node, Direction.West);
        GUILayout.Space(20);
        if (GUILayout.Button("X East", GUILayout.Width(80))) 
            ApplyDisconnect(node, Direction.East);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Bottom Row (South)
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("X South", GUILayout.Width(80))) 
            ApplyDisconnect(node, Direction.South);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Delete Self", GUILayout.Width(80)))
            node.DeleteSelf();
    }

    private void ApplyDisconnect(Node node, Direction dir)
    {
        Undo.RecordObject(node, "Disconnect Node");
        Node neighbor = node.GetNodeInDirection(dir);
        if (neighbor != null) 
        {
            Undo.RecordObject(neighbor, "Disconnect Node");
            EditorUtility.SetDirty(neighbor); // Save the neighbor too!
        }
        node.RemoveNeighbour(dir);
        EditorUtility.SetDirty(node);
    }
}
