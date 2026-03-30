using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private BoardInspectCamera _camScript;

    public void GenerateMap(LevelData data)
    {
        GameObject currentMap = Instantiate(data.MapPrefab);

        void AssignCameraFocusPoint(GameObject o)
        {
            Transform focusPoint = o.transform.Find("FocusPoint");
            if (focusPoint != null && _camScript != null)
            {
                _camScript.AssignFocusPoint(focusPoint);
            }
            else
            {
                Debug.LogError("Where?");
            }
        }

        AssignCameraFocusPoint(currentMap);
    }
}