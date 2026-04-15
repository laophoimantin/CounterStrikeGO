using System;
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

    [Header("C4")]
    [SerializeField] private GameObject _c4Model;

    [SerializeField] private Transform _modelPivot;
    private Transform _cameraTransform;

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerPickedUp += TurnC4Model;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerPickedUp -= TurnC4Model;
    }

    void Start()
    {
        if (Camera.main != null)
            _cameraTransform = Camera.main.transform;
        SetHoldingItemState(false);
        
        _c4Model.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_cameraTransform == null) return;

        if (_modelPivot.parent != null)
        {
            Vector3 camLocalPos = _modelPivot.parent.InverseTransformPoint(_cameraTransform.position);

            camLocalPos.y = 0;

            if (camLocalPos != Vector3.zero)
            {
                _modelPivot.localRotation = Quaternion.LookRotation(camLocalPos, Vector3.up);
            }
        }
        else
        {
            Vector3 directionToCamera = _cameraTransform.position - _modelPivot.position;
            directionToCamera.y = 0;
            if (directionToCamera != Vector3.zero)
            {
                _modelPivot.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
    }

    // Utility ===========================================
    public void SetHoldingItemState(bool hasUtility)
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

    private void TurnC4Model()
    {
        _c4Model.SetActive(true);
    }
}