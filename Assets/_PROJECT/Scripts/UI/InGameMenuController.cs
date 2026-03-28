using UnityEngine;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectivesPanel _objectivesPanel;
    [SerializeField] private string _mainMenuScene = "MainMenu";

    [Header("Buttons")]
    [SerializeField] private Button _openMenuButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _hintButton;


    void Awake()
    {
        _openMenuButton.onClick.RemoveAllListeners();
        _resetButton.onClick.RemoveAllListeners();
        _hintButton.onClick.RemoveAllListeners();

        _openMenuButton.onClick.AddListener(OpenMenu);
        _resetButton.onClick.AddListener(ResetGame);
        _hintButton.onClick.AddListener(ShowHint);
    }

    private void OpenMenu()
    {
        _objectivesPanel.OpenPausePanel();
    }

    private void ResetGame()
    {
        SceneController.Instance.ReloadCurrentScene();
    }

    private void Exit()
    {
        SceneController.Instance.LoadNewScene(_mainMenuScene);
    }

    private void ShowHint()
    {
        Debug.Log("Hint: Stop ghosting your variables.");
    }
}