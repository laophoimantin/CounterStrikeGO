using DG.Tweening;
using UnityEngine;

public class PlayerVisual : GridUnitVisual
{
    [Header("Player Model")]
    [SerializeField] private Transform _normalStateModel;
    [SerializeField] private Transform _usingUtilityModel;

    [Header("PickUp Animation")]
    [SerializeField] private float _liftHeight;
    [SerializeField] private float _liftDuration;

    void Start()
    {
        SetUsingUtilityVisible(false);
    }

    // public override Tween FlyUp()
    // {
    //     Sequence deathSeq = DOTween.Sequence();
    //         
    //     deathSeq.Join(_baseModel.DORotate(new Vector3(0, 360, 0), _flyDuration, RotateMode.FastBeyond360));
    //     deathSeq.Join(_baseModel.DOScale(Vector3.zero, _flyDuration).SetEase(Ease.InBack));
    //         
    //     return deathSeq;
    // }

    // Utility ===========================================
    public void SetUsingUtilityVisible(bool hasUtility)
    {
        _normalStateModel.gameObject.SetActive(!hasUtility);
        _usingUtilityModel.gameObject.SetActive(hasUtility);
    }

    // Pickup ============================================
    public void PickUpAnim()
    {
        _baseModel.DOKill();
        _baseModel.DOLocalMoveY(_liftHeight, _liftDuration).SetEase(Ease.OutBack);
    }

    public void DropAnim()
    {
        _baseModel.DOKill();
        _baseModel.DOLocalMoveY(0f, _liftDuration).SetEase(Ease.OutBounce);
    }
}