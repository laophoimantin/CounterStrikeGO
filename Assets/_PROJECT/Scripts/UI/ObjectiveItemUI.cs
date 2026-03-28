using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _description;
    
    [Header("UI Panels")]
    [SerializeField] private RectTransform _mainPanel;
    
    [Header("Complete Mark")]
    [SerializeField] private RectTransform _completeMark;
    [SerializeField] private CanvasGroup _completeMarkGroup;

    [Header("Slide Settings")]
    [SerializeField] private float _animDuration = 1f;
    [SerializeField] private Ease _easeIn = Ease.InOutQuint;
    [SerializeField] private Ease _easeOut = Ease.InOutQuint;
    
    [Header("Complete Mark Settings")]
    [SerializeField] private float _markDuration = 0.5f;
    [SerializeField] private Ease _markEase = Ease.InOutQuint;
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeStrength = 5f;

    private RuntimeObjective _data;
    private bool _wasAlreadyComplete;

    private Sequence _winSeq;
    
    public void Initialize(RuntimeObjective data)
    {
        _data = data;
        _icon.sprite = data.Blueprint.Icon;
        _description.text = data.Blueprint.Description;

        _wasAlreadyComplete = data.IsCompleteNow;
        
        _completeMark.gameObject.SetActive(_wasAlreadyComplete);
        _completeMarkGroup.alpha = _wasAlreadyComplete ? 1f : 0f;
    }

    public Tween GetSlideInTween(float offScreenYPos)
    {
        KillAllTweens();
        _mainPanel.gameObject.SetActive(true);
        _mainPanel.anchoredPosition = new Vector2(_mainPanel.anchoredPosition.x, offScreenYPos);

        return _mainPanel.DOAnchorPosY(0, _animDuration).SetEase(_easeIn);
    }

    public Tween GetSlideOutTween(float offScreenYPos)
    {
        KillAllTweens();
        return _mainPanel.DOAnchorPosY(offScreenYPos, _animDuration).SetEase(_easeOut);
    }

    public Sequence GetWinAnimationSequence(float offScreenYPos)
    {
        _winSeq?.Kill();
        KillAllTweens();
        
        _mainPanel.gameObject.SetActive(true);
        _mainPanel.anchoredPosition = new Vector2(_mainPanel.anchoredPosition.x, offScreenYPos);
        
        _winSeq = DOTween.Sequence();
        
        _winSeq.Append(_mainPanel.DOAnchorPosY(0, _animDuration).SetEase(_easeIn));
        
        if (!_wasAlreadyComplete && _data.IsCompleteNow)
        {
            _winSeq.Append(MarkAnimation());
        }

        return _winSeq;
    }

    private Sequence MarkAnimation()
    {
        Sequence markSeq = DOTween.Sequence();
        _completeMark.gameObject.SetActive(true);
        _completeMark.localScale = Vector3.one; 
        _completeMarkGroup.alpha = 1f;

        markSeq.Append(_completeMark.DOScale(Vector3.one * 2, _markDuration).From().SetEase(_markEase));
        markSeq.Join(_completeMarkGroup.DOFade(0f, _markDuration).From().SetEase(_markEase));
        markSeq.Append(_completeMark.DOShakeAnchorPos(_shakeDuration, _shakeStrength));

        return markSeq;
    }
    
    private void KillAllTweens()
    {
        _mainPanel.DOKill();
        _completeMark.DOKill();     
        _completeMarkGroup.DOKill();
    }
}