using System;
using System.Collections;
using Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    
    private bool _isLoading;
    
    [Header("Settings")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private bool _dontDestroyOnLoad = true;

    [Header("References")]
    [SerializeField] private EventDispatcher _eventDispatcher;
    
    [Header("Loading Screen")]
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _progressBar;

    public static event Action OnSceneLoadStarted;
    public static event Action OnSceneLoadCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (_loadingScreen != null)
            _loadingScreen.SetActive(false);
    }

    // lmao
    #region Public API

    /// Standard Level Transition: Fades out, loads a new scene, fades in.
    public void LoadNewScene(string sceneName)
    {
        LoadScene(sceneName);
    }

    public void LoadGameplayScene()
    {
        LoadScene(SceneName.Gameplay);
    }
    public void LoadMainMenu()
    {
        LoadScene(SceneName.MainMenu);
    }
    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }
    

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    #endregion

    // lmao

    #region Internal Logic

    private void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        _isLoading = true;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
    

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        OnSceneLoadStarted?.Invoke();

        _canvasGroup.blocksRaycasts = true;

        // PHASE 1: TRANSITION TO LOADING SCREEN
        // =============================================================================

        yield return Fade(0f, 1f);

        if (_loadingScreen != null)
            _loadingScreen.SetActive(true);

        yield return Fade(1f, 0f);

        // PHASE 2: LOADING
        // =============================================================================

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        _eventDispatcher.Clear();
        operation.allowSceneActivation = false; // Prevent auto-jumping

        // While loading...
        while (operation.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (_progressBar != null) 
                _progressBar.value = progress;
            yield return null;
        }

        if (_progressBar != null) 
            _progressBar.value = 1f;

        yield return new WaitForSecondsRealtime(0.5f);

        // PHASE 3: TRANSITION TO NEW SCENE
        // =============================================================================

        yield return Fade(0f, 1f);

        operation.allowSceneActivation = true;

        while (!operation.isDone) 
            yield return null;

        if (_loadingScreen != null)
            _loadingScreen.SetActive(false);

        yield return Fade(1f, 0f);

        _canvasGroup.blocksRaycasts = false;
        OnSceneLoadCompleted?.Invoke();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        _canvasGroup.alpha = startAlpha;

        while (elapsed < _fadeDuration)
        {
            // Use unscaled time so the fade works even if the game is paused
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = endAlpha;
    }

    #endregion
}

public static class SceneName
{
    public const string MainMenu = "MainMenu";
    public const string Gameplay = "Gameplay";
}