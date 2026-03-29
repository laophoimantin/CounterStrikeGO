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


    void Awake()
    {
        _openMenuButton.onClick.AddListener(OpenMenu);
        _resetButton.onClick.AddListener(ResetGame);
    }

    private void OpenMenu()
    {
        _objectivesPanel.OpenPausePanel();
    }

    private void ResetGame()
    {
        SceneController.Instance.ReloadCurrentScene();
    }
}