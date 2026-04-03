using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private BoardInspectCamera _camScript;

    public void GenerateMap(LevelData data)
    {
        GameObject currentMap = Instantiate(data.MapPrefab);

        MapController mapCtrl = currentMap.GetComponent<MapController>();

        if (mapCtrl != null && mapCtrl.DefaultFocusPoint != null && _camScript != null)
        {
            
            _camScript.AssignFocusPoint(mapCtrl.DefaultFocusPoint);
        }
    }
}