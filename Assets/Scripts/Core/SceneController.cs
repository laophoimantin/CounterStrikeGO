using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [Tooltip("If true, this object will survive scene changes.")]
        [SerializeField] private bool _dontDestroyOnLoad = true;

        [Header("References")]
        [SerializeField] private GameObject _loadingScreenCanvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider _progressBar; 

        public event Action OnSceneLoadStarted;
        public event Action OnSceneLoadCompleted;

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

            if (_canvasGroup == null) 
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_loadingScreenCanvas != null) 
                _loadingScreenCanvas.SetActive(false);
        }
        
        // lmao
        #region Public API 

        /// Standard Level Transition: Fades out, loads new scene, fades in.
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
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

        private IEnumerator LoadSceneRoutine(string sceneName)
        {
            OnSceneLoadStarted?.Invoke();
            
            // 1. Block Input & Fade In
            _loadingScreenCanvas.SetActive(true);
            _canvasGroup.blocksRaycasts = true; // Stop players from clicking buttons while fading
            yield return Fade(0f, 1f);

            // 2. Load Scene
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            
            // click to continue option
            // operation.allowSceneActivation = false; 

            while (operation is { isDone: false })
            {
                // Unity's progress stops at 0.9f
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                if (_progressBar != null) 
                    _progressBar.value = progress;

                yield return null;
            }

            // 3. Wait a tiny bit for initialization
            yield return new WaitForSecondsRealtime(0.2f);

            // 4. Fade Out & Restore Input
            yield return Fade(1f, 0f);
            
            _canvasGroup.blocksRaycasts = false;
            _loadingScreenCanvas.SetActive(false);
            
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
}