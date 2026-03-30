using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Top Panel")]
    [SerializeField] private Button _mainButton;

    [Header("Level Selection Panel")]
    [SerializeField] private RectTransform _levelSelectionPanel;
    [SerializeField] private Button _returnButton;

    [Space(10)]
    [SerializeField] private RectTransform _bottomArea;

    [Header("Animation Settings")]
    [SerializeField] private float _animDuration = 0.5f;

    private float _panelHeight;
    private Vector2 _selectionOriginalPos;

    private float _bottomHeight;
    private Vector2 _bottomOriginalPos;

    private Sequence _panelSeq;

    void Awake()
    {
        _mainButton.onClick.AddListener(ShowPanel);
        _returnButton.onClick.AddListener(HidePanel);
    }

    void Start()
    {
        UpdateLevelSelectOriginalPos();
        _levelSelectionPanel.anchoredPosition = _selectionOriginalPos;

        UpdateBottomAreaOriginalPos();
        _bottomArea.anchoredPosition = _bottomOriginalPos;
    }

    private void ShowPanel()
    {
        _panelSeq?.Kill();
        _panelSeq = DOTween.Sequence();
        UpdateOriPos();
        _panelSeq.Append(MovePanelAlongY(_levelSelectionPanel, 0));
        _panelSeq.Join(MovePanelAlongY(_bottomArea, 0));
    }

    private void HidePanel()
    {
        _panelSeq?.Kill();
        _panelSeq = DOTween.Sequence();
        UpdateOriPos();
        _panelSeq.Append(MovePanelAlongY(_levelSelectionPanel, _selectionOriginalPos.y));
        _panelSeq.Join(MovePanelAlongY(_bottomArea, _bottomOriginalPos.y));
    }

    private Tween MovePanelAlongY(RectTransform panel, float targetPos)
    {
        return panel.DOAnchorPosY(targetPos, _animDuration).SetEase(Ease.OutCirc);
    }


    private void UpdateOriPos() // Counter the screen size changing midplay 
    {
        UpdateLevelSelectOriginalPos();
        UpdateBottomAreaOriginalPos();
    }

    private void UpdateLevelSelectOriginalPos()
    {
        _panelHeight = _levelSelectionPanel.rect.height;
        _selectionOriginalPos = new Vector2(0, _panelHeight);
    }

    private void UpdateBottomAreaOriginalPos()
    {
        _bottomHeight = _bottomArea.rect.height;
        _bottomOriginalPos = new Vector2(0, -_bottomHeight);
    }
}