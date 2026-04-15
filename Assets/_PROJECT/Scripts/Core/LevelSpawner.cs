using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private BoardInspectCamera _camScript;

    public void GenerateMap(LevelData data)
    {
        GameObject currentMap = Instantiate(data.MapPrefab);
        currentMap.SetActive(true);
    }
}