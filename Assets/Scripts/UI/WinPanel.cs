using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core.Events;

public class WinPanel : MonoBehaviour // Removed Singleton
{
    [Header("UI Elements")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private RectTransform _buttonContainer;
    [SerializeField] private CanvasGroup _backgroundGroup;
    
    [Header("Buttons")]
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _nextLevelButton;

    [Header("Animation Settings")]
    [SerializeField] private float _animDuration = 0.5f;
    [SerializeField] private float _startYOffset = -500f; 

    private Vector2 _originalPos;

    void Awake()
    {
        _originalPos = _buttonContainer.anchoredPosition;

        _resetButton.onClick.AddListener(OnResetClicked);
        _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        
        _panelRoot.SetActive(false);
    }

    void OnEnable()
    {
        this.Subscribe<OnGameEndedEvent>(Show);
    }

    private void OnDisable()
    {
        this.Unsubscribe<OnGameEndedEvent>(Show);
    }

    private void Update()
    {
 
    }

    private void Show(OnGameEndedEvent eventData)
    {
        _panelRoot.SetActive(true);

        // Move buttons down 
        _buttonContainer.anchoredPosition = new Vector2(_originalPos.x, _startYOffset);
        
        if (_backgroundGroup != null) 
            _backgroundGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        if (_backgroundGroup != null)
            seq.Join(_backgroundGroup.DOFade(1f, _animDuration)); 

        seq.Join(_buttonContainer.DOAnchorPos(_originalPos, _animDuration).SetEase(Ease.OutBack));
    }

    private void OnResetClicked()
    {
        _resetButton.interactable = false; 
        SceneController.Instance.ReloadCurrentScene();
    }

    private void OnNextLevelClicked()
    {
        _nextLevelButton.interactable = false;
        Debug.Log("Next Level Not Implemented Yet!");
    }
}