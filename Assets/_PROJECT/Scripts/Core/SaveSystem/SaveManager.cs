using System.Collections.Generic;
using System.IO;
using Core.Patterns;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    //[SerializeField] private string _saveFilePath = Path.Combine(Application.persistentDataPath, "hitman_wizards_save.json");
    [SerializeField] private string _saveFilePath;

    // Đây là trí nhớ ngắn hạn của game. Mọi truy xuất lúc đang chơi sẽ lấy từ đây.
    public GameSaveData CurrentData { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        LoadGame(); // Vừa bật game lên là bắt thằng thủ kho đi kiểm kê hàng hóa ngay!
    }

    // --- BƯỚC 1: LOAD (Lấy từ ổ cứng lên RAM) ---
    public void LoadGame()
    {
        if (File.Exists(_saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(_saveFilePath);
                // Ma thuật của Newtonsoft đây:
                CurrentData = JsonConvert.DeserializeObject<GameSaveData>(json);
                //Debug.Log("Load Save File thành công!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"File save bị thiu rồi: {e.Message}");
                CreateNewSave();
            }
        }
        else
        {
            Debug.Log("Chưa có file save, tạo mới!");
            CreateNewSave();
        }
    }

    private void CreateNewSave()
    {
        CurrentData = new GameSaveData();
        // Mở khóa sẵn level 1 cho người chơi khỏi chửi
        CurrentData.Levels["level_1"] = new LevelSaveData { IsUnlocked = true };
        SaveGame(); // Ép lưu ngay lập tức để tạo file vật lý
    }

    // --- BƯỚC 2: GIAO TIẾP VỚI BÊN NGOÀI (Lấy & Ghi vào RAM) ---

    public bool IsObjectiveComplete(string levelId, string objectiveId)
    {
        if (CurrentData.Levels.TryGetValue(levelId, out LevelSaveData levelData))
        {
            if (levelData.ObjectiveStates.TryGetValue(objectiveId, out bool isComplete))
            {
                return isComplete; // Trả về true/false từ file save
            }
        }

        return false; // Mặc định chưa hoàn thành
    }

    public void MarkObjectiveComplete(string levelId, string objectiveId)
    {
        // Nếu level chưa tồn tại trong save thì đẻ ra nó
        if (!CurrentData.Levels.ContainsKey(levelId))
            CurrentData.Levels[levelId] = new LevelSaveData();

        CurrentData.Levels[levelId].ObjectiveStates[objectiveId] = true;
    }

    // --- BƯỚC 3: SAVE (Ghi từ RAM xuống ổ cứng) ---
    public void SaveGame()
    {
        try
        {
            // Formatting.Indented để file JSON mở ra đọc bằng mắt người cho đẹp lúc Debug
            string json = JsonConvert.SerializeObject(CurrentData, Formatting.Indented);
            File.WriteAllText(_saveFilePath, json);
            Debug.Log("Đã lưu game vào két sắt!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lưu game thất bại: {e.Message}");
        }
    }
}

[System.Serializable]
public class GameSaveData
{
    // Key: LevelId (ví dụ: "level_1"), Value: Dữ liệu của level đó
    public Dictionary<string, LevelSaveData> Levels = new Dictionary<string, LevelSaveData>();

    // Sau này ông muốn nhét thêm Settings (Âm thanh, Đồ họa) hay Inventory thì nhét vào đây!
    // public PlayerSettings Settings = new PlayerSettings(); 
}

[System.Serializable]
public class LevelSaveData
{
    public bool IsUnlocked;

    // Key: ObjectiveId (ví dụ: "kill_boss"), Value: Đã hoàn thành chưa (true/false)
    // Tạm biệt cái List ngu học O(N) của ông nhé!
    public Dictionary<string, bool> ObjectiveStates = new Dictionary<string, bool>();
}