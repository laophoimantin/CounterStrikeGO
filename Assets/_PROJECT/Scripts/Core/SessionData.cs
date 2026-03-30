using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SessionData
{
    public static LevelData CurrentLevelData { get; private set; }
    public static LevelData NextLevelDataToLoad => CurrentLevelData?.NextLevel;
    
    public static void SetCurrentLevelData(LevelData data) => CurrentLevelData = data;
}