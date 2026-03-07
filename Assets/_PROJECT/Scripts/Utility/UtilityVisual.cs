using System.Collections;
using DG.Tweening;
using UnityEngine;

public abstract class UtilityVisual : GridUnitVisual
{
    [SerializeField] protected Transform _utilityModel;
    
    [Header("Throw Animation")]
    [SerializeField] private float _throwHeight = 2;
    [SerializeField] private float _throwDuration = 1;

    public virtual void Start()
    {
        if (_utilityModel != null)
            _utilityModel.gameObject.SetActive(false);
        if (_pawnModel != null)
            _pawnModel.gameObject.SetActive(true);
    }

    public void SwitchToFlyingMode(Vector3 startPos)
    {
        _pawnModel.gameObject.SetActive(false);
        _utilityModel.gameObject.SetActive(true);
        _utilityModel.transform.position = startPos + Vector3.up * 2;
    }

    public void HideUtilityModel()
    {
        _utilityModel.gameObject.SetActive(false);
    }

    public Sequence GetThrowSequence(Vector3 targetPos)
    {
        Sequence throwSeq = DOTween.Sequence();

        throwSeq.Append(_utilityModel.transform
            .DOJump(targetPos, _throwHeight, 1, _throwDuration)
            .SetEase(Ease.Linear));

        throwSeq.Join(_utilityModel.transform
            .DORotate(new Vector3(360f, 0, 0), _throwDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear));

        return throwSeq;
    }

    public abstract Tween GetLandedAnim();
}