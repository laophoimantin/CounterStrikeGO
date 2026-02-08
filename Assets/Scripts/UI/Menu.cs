using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    
    public bool IsOpen => _menuPanel.activeSelf; 

    private void Start()
    {
        _menuPanel.SetActive(false);
    }

    public void OpenMenu()
    {
        _menuPanel.SetActive(true);
        // TODO: Blur background
    }

    public void CloseMenu()
    {
        _menuPanel.SetActive(false);
        // TODO: UnBlur background
    }
    
    public void Toggle()
    {
        if (IsOpen) CloseMenu();
        else OpenMenu();
    }
}