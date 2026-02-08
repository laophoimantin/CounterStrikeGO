using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ButtonSwapper _swapper;

    [SerializeField] private Menu _menu;
    [SerializeField] private string _mainMenuScene = "MainMenu";

    [Header("Buttons")]
    [SerializeField] private Button _openMenuButton;
    [SerializeField] private Button _closeMenuButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _hintButton;
    [SerializeField] private Button _exitButton;


    void Start()
    {
        if (_openMenuButton != null) _openMenuButton.onClick.AddListener(OpenMenu);
        if (_closeMenuButton != null) _closeMenuButton.onClick.AddListener(CloseMenu);

        if (_resetButton != null) _resetButton.onClick.AddListener(ResetGame);
        if (_exitButton != null) _exitButton.onClick.AddListener(Exit);

        // Don't ghost your buttons!
        if (_hintButton != null) _hintButton.onClick.AddListener(ShowHint);
    }

    private void OpenMenu()
    {
        _menu.OpenMenu();
        _swapper.Swap(showMenuVersion: true);
    }

    private void CloseMenu()
    {
        _menu.CloseMenu();
        _swapper.Swap(showMenuVersion: false);
    }

    private void ResetGame()
    {
        SceneController.Instance.ReloadCurrentScene();
    }

    private void Exit()
    {
        SceneController.Instance.LoadScene(_mainMenuScene);
    }

    private void ShowHint()
    {
        Debug.Log("Hint: Stop ghosting your variables.");
    }
}