using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private const string SAVE_DIR_NAME = "SaveFiles";
    private const string SAVE_FILE_NAME = "PlayerSave.json";

    private string SaveDirectory => Path.Combine(Application.persistentDataPath, SAVE_DIR_NAME);
    private string SaveFileName => Path.Combine(SaveDirectory, SAVE_FILE_NAME);

    public GameSaveData CurrentData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        LoadSaveFile();
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Clear Player Save")]
    public static void DeleteSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_DIR_NAME, SAVE_FILE_NAME);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
#endif

    private void LoadSaveFile()
    {
        if (TryLoadPlayerSave())
        {
            Debug.Log("Old save loaded successfully!");
            return;
        }

        Debug.Log("No save found, created new!");
        CreateNewSave();
    }

    private bool TryLoadPlayerSave()
    {
        if (!File.Exists(SaveFileName)) return false;

        try
        {
            string json = File.ReadAllText(SaveFileName);
            var data = JsonConvert.DeserializeObject<GameSaveData>(json);

            if (data == null)
            {
                Debug.LogWarning("File exists but deserialize returned null");
                return false;
            }

            CurrentData = data;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Player save is corrupted: {e.Message}");
            return false;
        }
    }

    private void CreateNewSave()
    {
        CurrentData = new GameSaveData();
        SaveGame();
    }


    // Objective completion
    public bool IsObjectiveComplete(string levelId, string objectiveId)
    {
        var level = GetLevelData(levelId);
        return level != null && level.completedObjectiveIds.Contains(objectiveId);
    }

    public void SetObjectiveComplete(string levelId, string objectiveId)
    {
        var level = GetOrCreateLevelData(levelId);
        level.completedObjectiveIds.Add(objectiveId);
    }

    // Level unlock
    public bool IsLevelUnlocked(string levelId, bool isUnlockedByDefault = false)
    {
        var level = GetOrCreateLevelData(levelId, isUnlockedByDefault);
        return level.isUnlocked;
    }

    public void SetLevelUnlocked(string levelId)
    {
        var level = GetOrCreateLevelData(levelId);
        level.isUnlocked = true;
    }

    // Helpers
    private LevelSaveData GetLevelData(string levelId)
    {
        if (CurrentData.levels.TryGetValue(levelId, out var level))
        {
            return level;
        }

        return null;
    }

    private LevelSaveData GetOrCreateLevelData(string levelId, bool isUnlockedByDefault = false)
    {
        if (!CurrentData.levels.TryGetValue(levelId, out var level))
        {
            level = new LevelSaveData { isUnlocked = isUnlockedByDefault };
            CurrentData.levels.Add(levelId, level);
        }

        return level;
    }

    public void SaveGame()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        string json = JsonConvert.SerializeObject(CurrentData, Formatting.Indented);
        File.WriteAllText(SaveFileName, json);
    }
}

[Serializable]
public class GameSaveData
{
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
    public Dictionary<string, LevelSaveData> levels = new Dictionary<string, LevelSaveData>();
}

[Serializable]
public class LevelSaveData
{
    public bool isUnlocked;
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
    public HashSet<string> completedObjectiveIds = new();
}