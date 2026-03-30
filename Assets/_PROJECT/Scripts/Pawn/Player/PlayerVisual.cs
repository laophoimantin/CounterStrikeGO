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