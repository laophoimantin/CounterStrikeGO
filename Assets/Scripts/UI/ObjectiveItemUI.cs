using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveItemUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private RectTransform _completeIcon;
    [SerializeField] private RectTransform _incompleteIcon;
    [SerializeField] private RectTransform _container;

    [Header("Animation Settings")]
    [SerializeField] private float _itemStaggerDelay = 0.1f;
    [SerializeField] private float _animDuration = 1f;
    [SerializeField] private Ease _easeIn = Ease.InOutQuint;
    [SerializeField] private Ease _easeOut = Ease.InOutQuint;

    private RuntimeObjective _data;

    public void Initialize(RuntimeObjective data)
    {
        _data = data;
        _icon.sprite = data.Blueprint.Icon;

        _description.text = data.Blueprint.Description;

        if (data.IsCompleteBefore)
        {
            _completeIcon.gameObject.SetActive(true);
            _incompleteIcon.gameObject.SetActive(false);
        }
        else
        {
            _completeIcon.gameObject.SetActive(false);
            _incompleteIcon.gameObject.SetActive(true);
        }
    }

    public void Refresh()
    {
        bool complete = _data.IsCompleteNow || _data.IsCompleteBefore;
        _completeIcon.gameObject.SetActive(complete);
        _incompleteIcon.gameObject.SetActive(!complete);
    }

    public void SlideIn(Sequence seq, int i, float offScreenYPos)
    {
        _container.DOKill();
        _container.gameObject.SetActive(true);

        float targetOnScreenY = 0;

        _container.anchoredPosition = new Vector2(_container.anchoredPosition.x, offScreenYPos);

        seq.Insert(i * _itemStaggerDelay,
            _container.DOAnchorPosY(targetOnScreenY, _animDuration)
                .SetEase(_easeIn));
    }

    public void SlideOut(Sequence seq, int i, float offScreenYPos)
    {
        _container.DOKill();

        seq.Insert(i * _itemStaggerDelay,
            _container.DOAnchorPosY(offScreenYPos, _animDuration)
                .SetEase(_easeOut));
    }
}