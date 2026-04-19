using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    private bool _isUnlocked;

    [Header("Components")]
    [SerializeField] private Button _button;
    [Header("Visual Elements")]
    [SerializeField] private Image _buttonImage;
    [SerializeField] private TextMeshProUGUI _levelIndexText;
    [SerializeField] private Color _lockedColor = Color.grey;
    [SerializeField] private Color _unlockedColor = Color.black;

    [Header("Level Data")]
    [SerializeField] private LevelData _levelData;

    [Space(10)]
    [SerializeField] private Image _mainObjectiveIconImage; // always active
    [SerializeField] private Image[] _optionalObjectiveIconSpriteImage;

    void Awake()
    {
        _button.onClick.AddListener(LoadLevel);
        foreach (Image icon in _optionalObjectiveIconSpriteImage)
        {
            icon.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        this.Subscribe<OnSaveDataChangedEvent>(OnSaveDataChanged);
    }

    private void OnDisable()
    {
        this.Unsubscribe<OnSaveDataChangedEvent>(OnSaveDataChanged);
    }

    private void OnSaveDataChanged(OnSaveDataChangedEvent evt)
    {
        UpdateVisuals();
    }

    void Start()
    {
        _levelIndexText.text = $"{_levelData.LevelDisplayNumber}";

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        _isUnlocked = SaveManager.Instance.IsLevelUnlocked(_levelData.LevelId, _levelData.IsUnlockedByDefault);

        _button.interactable = _isUnlocked;
        _buttonImage.color = _isUnlocked ? _unlockedColor : _lockedColor;

        bool isMainDone = SaveManager.Instance.IsObjectiveComplete(_levelData.LevelId, _levelData.MainObjective.Id);
        SetObjectiveIconsSprite(_mainObjectiveIconImage, isMainDone);

        for (int i = 0; i < _levelData.OptionalObjectives.Count; i++)
        {
            _optionalObjectiveIconSpriteImage[i].gameObject.SetActive(true);
            bool isOptDone = SaveManager.Instance.IsObjectiveComplete(
                _levelData.LevelId,
                _levelData.OptionalObjectives[i].Id
            );

            SetObjectiveIconsSprite(_optionalObjectiveIconSpriteImage[i], isOptDone);
        }
    }
    private void LoadLevel()
    {
        SessionData.SetCurrentLevelData(_levelData);
        //SceneController.Instance.LoadNewScene(_levelData.LevelId);
        SceneController.Instance.LoadGameplayScene();
    }

    void SetObjectiveIconsSprite(Image image, bool isComplete)
    {
        image.gameObject.SetActive(true);
        Color color = image.color;
        color.a = isComplete ? 1f : 0.5f;
        image.color = color;
    }
}