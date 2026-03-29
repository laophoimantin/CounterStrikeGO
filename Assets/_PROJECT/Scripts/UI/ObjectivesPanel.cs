using System.Collections.Generic;
using Core;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesPanel : MonoBehaviour
{
    [Header("Main UI Elements")]
    [SerializeField] private RectTransform _mainPanel;
    [SerializeField] private RectTransform _buttonContainer;
    [SerializeField] private CanvasGroup _backgroundGroup;

    [Header("Pause")]
    [SerializeField] private RectTransform _pauseButtonContainer;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _pauseResetButton;
    [SerializeField] private Button _exitButton;

    [Header("Win")]
    [SerializeField] private RectTransform _winButtonContainer;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _winResetButton;

    [Header("Objectives")]
    [SerializeField] private RectTransform _objectiveContainer;
    [SerializeField] private ObjectiveItemUI _objectiveItemUIPrefab;
    private readonly List<ObjectiveItemUI> _spawnedItems = new();

    [Header("Animation Settings")]
    [SerializeField] private float _itemStaggerDelay = 0.3f;
    [SerializeField] private float _animDuration = 0.5f;
    [SerializeField] private float _startYOffset = -500f;
    private Vector2 _originalPos;
    private Vector2 _hiddenPos;

    [SerializeField] private Ease _easeOpen = Ease.InOutQuint;
    [SerializeField] private Ease _easeClose = Ease.InOutQuint;

    [SerializeField] private RectTransform _offScreenPos;
    private float OffScreenYPos => _offScreenPos.anchoredPosition.y;

    private Sequence _panelSeq;

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
        _returnButton.onClick.AddListener(OnReturnClicked);
        _pauseResetButton.onClick.AddListener(OnResetClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
        _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        _winResetButton.onClick.AddListener(OnResetClicked);

        _originalPos = _buttonContainer.anchoredPosition;
        _hiddenPos = new Vector2(_originalPos.x, _startYOffset);
    }

    public void Initialize(RuntimeObjective main, List<RuntimeObjective> optionals)
    {
        Clear();
        SpawnObjective(main);
        foreach (var obj in optionals)
            SpawnObjective(obj);
        EnableButtons();
    }

    private void Clear()
    {
        _panelSeq?.Kill();

        foreach (var item in _spawnedItems)
            Destroy(item.gameObject);

        _spawnedItems.Clear();

        _mainPanel.gameObject.SetActive(false);
        _pauseButtonContainer.gameObject.SetActive(false);
        _winButtonContainer.gameObject.SetActive(false);
    }

    private void SpawnObjective(RuntimeObjective objective)
    {
        var item = Instantiate(_objectiveItemUIPrefab, _objectiveContainer);
        item.Initialize(objective);
        _spawnedItems.Add(item);
    }

    private void CreateNewSequence()
    {
        _panelSeq?.Kill();
        _panelSeq = DOTween.Sequence();
    }

    private void Open()
    {
        OpenPanelBase();
        for (int i = 0; i < _spawnedItems.Count; i++)
            _panelSeq.Insert(i * _itemStaggerDelay, _spawnedItems[i].GetSlideInTween(OffScreenYPos));
    }

    private void OpenWin()
    {
        OpenPanelBase();
        for (int i = 0; i < _spawnedItems.Count; i++)
            _panelSeq.Append(_spawnedItems[i].GetWinAnimationSequence(OffScreenYPos));
    }

    private void OpenPanelBase()
    {
        _mainPanel.gameObject.SetActive(true);
        _buttonContainer.DOKill();
        _backgroundGroup.DOKill();

        _buttonContainer.anchoredPosition = _hiddenPos;
        _backgroundGroup.alpha = 0f;

        CreateNewSequence();

        _panelSeq.Join(_backgroundGroup.DOFade(1f, _animDuration));
        _panelSeq.Join(_buttonContainer.DOAnchorPos(_originalPos, _animDuration).SetEase(_easeOpen));
    }

    private void Close()
    {
        _buttonContainer.DOKill();
        _backgroundGroup.DOKill();

        _buttonContainer.anchoredPosition = _originalPos;
        _backgroundGroup.alpha = 1f;

        CreateNewSequence();

        _panelSeq.Join(_backgroundGroup.DOFade(0f, _animDuration));
        _panelSeq.Join(_buttonContainer.DOAnchorPos(_hiddenPos, _animDuration).SetEase(_easeClose));

        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            _panelSeq.Insert(i * _itemStaggerDelay, _spawnedItems[i].GetSlideOutTween(OffScreenYPos));
        }

        _panelSeq.OnComplete(() =>
        {
            _mainPanel.gameObject.SetActive(false);
            _pauseButtonContainer.gameObject.SetActive(false);
        });
    }


    public void OpenPausePanel()
    {
        _winButtonContainer.gameObject.SetActive(false);
        _pauseButtonContainer.gameObject.SetActive(true);
        Open();
    }

    private void OpenWinPanel(OnGameEndedEvent eventData)
    {
        _pauseButtonContainer.gameObject.SetActive(false);
        _winButtonContainer.gameObject.SetActive(true);
        OpenWin();
    }

    private void OnReturnClicked()
    {
        Close();
    }

    private void OnExitClicked()
    {
        SceneController.Instance.LoadMainMenu();
    }

    private void OnResetClicked()
    {
        DisableButtons();
        SceneController.Instance.ReloadCurrentScene();
    }

    private void OnNextLevelClicked()
    {
        DisableButtons();
        GameManager.Instance.RequestNextLevel();
    }

    private void EnableButtons()
    {
        SetButtonInteractable(true);
    }

    private void DisableButtons()
    {
        SetButtonInteractable(false);
    }

    private void SetButtonInteractable(bool value)
    {
        _returnButton.interactable = value;
        _pauseResetButton.interactable = value;
        _winResetButton.interactable = value;
        _nextLevelButton.interactable = value;
        _exitButton.interactable = value;
    }
}