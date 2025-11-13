using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelBuilderManager : MonoBehaviour
{
    [SerializeField] private LevelBuilder m_LevelBuilder;

    public void SaveLevel()
    {
        // do save level logic

        // must have to save ScriptableObject changes
        //EditorUtility.SetDirty(mySO);
        Debug.Log("Save Successfully");
    }

    public void LoadLevel()
    {

    }
}
