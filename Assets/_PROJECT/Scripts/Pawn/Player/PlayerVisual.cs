using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerVisual : GridUnitVisual
{
    [Header("Utility Model")]
    [SerializeField] private GameObject _utilityModel;
    
    
    [Header("PickUp Animation")]
    [SerializeField] private float _liftHeight;
    [SerializeField] private float _liftDuration;

    void Start()
    {
        SwitchUtilityModel(false);
    }

    public override Tween FlyAnim()
    {
        Sequence deathSeq = DOTween.Sequence();
            
        deathSeq.Join(_pawnModel.DORotate(new Vector3(0, 360, 0), _duration, RotateMode.FastBeyond360));
        deathSeq.Join(_pawnModel.DOScale(Vector3.zero, _duration).SetEase(Ease.InBack));
            
        return deathSeq;
    }

    public void SwitchUtilityModel(bool hasUtility)
    {
        _pawnModel.gameObject.SetActive(!hasUtility);
        _utilityModel.SetActive(hasUtility);
    }
    
    public void PickedUpAnim()
    {
        _pawnModel.DOKill();
        _pawnModel.DOLocalMoveY(_liftHeight, _liftDuration).SetEase(Ease.OutBack);
    }
    
    public void DroppedAnim()
    {
        _pawnModel.DOKill();
        _pawnModel.DOLocalMoveY(0f, _liftDuration).SetEase(Ease.OutBounce);
    }
}
