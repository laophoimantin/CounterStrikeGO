using System.Collections.Generic;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesPanel : MonoBehaviour
{
    [SerializeField] private RectTransform _mainPanel;

    [SerializeField] private RectTransform _buttonContainer;
    [SerializeField] private CanvasGroup _backgroundGroup;

    [Header("Pause")]
    [SerializeField] private RectTransform _pauseButtonContainer;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private string _mainMenuScene = "MainMenu";

    [Header("Win")]
    [SerializeField] private RectTransform _winButtonContainer;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _resetButton2;


    [SerializeField] private RectTransform _objectiveContainer;
    [SerializeField] private ObjectiveItemUI _objectiveItemUIPrefab;
    private readonly List<ObjectiveItemUI> _spawnedItems = new();

    [Header("Animation Settings")]
    [SerializeField] private float _animDuration = 0.5f;
    [SerializeField] private float _startYOffset = -500f;
    private Vector2 _originalPos;
    private Vector2 _hiddenPos;

    [SerializeField] private Ease _easeOpen = Ease.InOutQuint;
    [SerializeField] private Ease _easeClose = Ease.InOutQuint;
    
    [SerializeField] private RectTransform _offScreenPos;
    public float OffScreenYPos => _offScreenPos.anchoredPosition.y;
    

    void OnEnable()
    {
        this.Subscribe<OnGameEndedEvent>(OpenWinPanel);
    }

    void OnDisable()
    {
        this.Unsubscribe<OnGameEndedEvent>(OpenWinPanel);
    }

    void Awake()
    {
        _returnButton.onClick.RemoveAllListeners();
        _resetButton.onClick.RemoveAllListeners();
        _exitButton.onClick.RemoveAllListeners();
        _nextLevelButton.onClick.RemoveAllListeners();
        _resetButton2.onClick.RemoveAllListeners();

        _returnButton.onClick.AddListener(OnReturnClicked);
        _resetButton.onClick.AddListener(OnResetClicked);
        _exitButton.onClick.AddListener(OnExitClicked);

        _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        _resetButton2.onClick.AddListener(OnResetClicked);

        _originalPos = _buttonContainer.anchoredPosition;
        _hiddenPos = new Vector2(_originalPos.x, _startYOffset);
    }

    public void Initialize()
    {
        // Hide
        _pauseButtonContainer.gameObject.SetActive(false);
        _winButtonContainer.gameObject.SetActive(false);
        _mainPanel.gameObject.SetActive(false);
    }

    public void SpawnObjective(RuntimeObjective objective)
    {
        var item = Instantiate(_objectiveItemUIPrefab, _objectiveContainer);
        item.Initialize(objective);
        _spawnedItems.Add(item);
    }

    private void Open()
    {
        _buttonContainer.DOKill();
        _backgroundGroup.DOKill();

        _buttonContainer.anchoredPosition = _hiddenPos;
        _backgroundGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Join(_backgroundGroup.DOFade(1f, _animDuration));
        seq.Join(_buttonContainer.DOAnchorPos(_originalPos, _animDuration).SetEase(_easeOpen));
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            _spawnedItems[i].SlideIn(seq,i, OffScreenYPos);
        }
    }

    private void Close()
    {
        _buttonContainer.DOKill();
        _backgroundGroup.DOKill();

        _buttonContainer.anchoredPosition = _originalPos;
        _backgroundGroup.alpha = 1f;

        Sequence seq = DOTween.Sequence();

        seq.Join(_backgroundGroup.DOFade(0f, _animDuration));
        seq.Join(_buttonContainer.DOAnchorPos(_hiddenPos, _animDuration).SetEase(_easeClose));

        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            _spawnedItems[i].SlideOut(seq, i, OffScreenYPos);
        }

        seq.OnComplete(() => {
            _mainPanel.gameObject.SetActive(false);
            _pauseButtonContainer.gameObject.SetActive(false);
        });
    }


    public void OpenPausePanel()
    {
        _mainPanel.gameObject.SetActive(true);
        _winButtonContainer.gameObject.SetActive(false);
        _pauseButtonContainer.gameObject.SetActive(true);
        Open();
    }

    private void ClosePause()
    {
        Close();
    }

    private void OnReturnClicked()
    {
        ClosePause();
    }

    private void OnExitClicked()
    {
        SceneController.Instance.LoadScene(_mainMenuScene);
    }


    private void OpenWinPanel(OnGameEndedEvent eventData)
    {
        _mainPanel.gameObject.SetActive(true);
        _pauseButtonContainer.gameObject.SetActive(false);
        _winButtonContainer.gameObject.SetActive(true);
        foreach (var item in _spawnedItems)
        {
            item.Refresh();
        }
        Open();
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