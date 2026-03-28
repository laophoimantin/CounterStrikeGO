using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayCamera : MonoBehaviour
{
    [SerializeField] private string _mainMenuSceneName = "MainMenu";
    
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        _camera.gameObject.SetActive(false);;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _camera.gameObject.SetActive(scene.name != _mainMenuSceneName);
      
    }
}
