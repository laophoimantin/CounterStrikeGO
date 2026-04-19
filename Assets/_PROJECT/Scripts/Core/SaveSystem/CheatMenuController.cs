using UnityEngine;
using UnityEngine.UI;

public class CheatMenuController : MonoBehaviour
{
    [SerializeField] private Button _btnUnlockAll;
    [SerializeField] private Button _btnResetSave;
    private void Start()
    {
        if (_btnUnlockAll != null)
        {
            _btnUnlockAll.onClick.AddListener(HandleUnlockAllClicked);
        }

        if (_btnResetSave != null)
        {
            _btnResetSave.onClick.AddListener(HandleResetSaveClicked);
        }
    }

    private void HandleUnlockAllClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.UnlockAllLevels();
        }
    }

    private void HandleResetSaveClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetSaveFile();
        }
    }
}