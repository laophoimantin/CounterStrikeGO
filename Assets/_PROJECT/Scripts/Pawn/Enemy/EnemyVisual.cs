using DG.Tweening;
using UnityEngine;

public class EnemyVisual : GridUnitVisual
{
    [Header("Enemy Model")]
    [SerializeField] private GameObject _normalStateModel;
    [SerializeField] private GameObject _flashedModel;

    [Header("Icon Animation")]
    [SerializeField] private float _iconDuration = 1f;
    [Header("Distracted")]
    [SerializeField] private Transform _questionIcon;


    [Header("Flashed")]
    [SerializeField] private Transform _flashedIcon;
    [SerializeField] private float _floatHeight = 0.2f;
    [SerializeField] private float _floatDuration = 0.5f;
    private float _iconStartY;


    void Start()
    {
        _normalStateModel.SetActive(true);
        _flashedModel.SetActive(false);
        _questionIcon.gameObject.SetActive(false);
        _flashedIcon.gameObject.SetActive(false);
        _iconStartY = _flashedIcon.localPosition.y;
    }

    // Marks =============================================================
    public Tween ShowQuestionIcon()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _questionIcon.gameObject.SetActive(true); });
        seq.Append(_questionIcon.DOLocalMoveY(1f, _iconDuration).From(true).SetEase(Ease.OutBounce));
        return seq;
    }

    public void HideQuestionIcon()
    {
        _questionIcon.DOKill();
        _questionIcon.gameObject.SetActive(false);
    }

    public void ShowStunIcon()
    {
        SwitchModel(true);
        _flashedIcon.gameObject.SetActive(true);

        _flashedIcon.DOKill();

        Vector3 resetPos = _flashedIcon.localPosition;
        resetPos.y = _iconStartY;
        _flashedIcon.localPosition = resetPos;

        _flashedIcon.DOLocalMoveY(_iconStartY + _floatHeight, _floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void HideStunIcon()
    {
        _flashedIcon.DOKill();
        SwitchModel(false);
        _flashedIcon.gameObject.SetActive(false);
    }

    private void SwitchModel(bool isFlashed)
    {
        _normalStateModel.SetActive(!isFlashed);
        _flashedModel.SetActive(isFlashed);
    }
}