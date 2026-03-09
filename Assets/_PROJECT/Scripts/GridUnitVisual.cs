using System;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class GridUnitVisual : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected Transform _pawnModel;
    [SerializeField] protected float _offScreenHeight = 15f;
    [SerializeField] protected float _duration = 1f;
    
    [SerializeField] private Vector3 _punchRotation = new Vector3(0, 0, 15f);
    [SerializeField] private float _wobbleDuration = 0.5f;
    [SerializeField] private int _vibrato = 10;
    [SerializeField] private float _elasticity = 1f;

    [SerializeField] private Vector3 _punchPosition = new (0, 0.2f, 0);
    [SerializeField] private float _landDuration = 0.3f;
    [SerializeField] private int _landVibrato = 8;
    protected virtual void Awake()
    {
        if (_pawnModel == null) 
            _pawnModel = transform.GetChild(0);
    }

    public Tween ShowBaseModel()
    {
        Sequence seq = DOTween.Sequence();

        _pawnModel.gameObject.SetActive(true);
        return seq;
    }

	public Tween MoveTo(Vector3 targetPos, float duration)
    {
        return transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
    }

    public Tween RotateTo(Quaternion targetRot, float duration)
    {
       return _pawnModel.DORotateQuaternion(targetRot, duration)
            .SetEase(Ease.OutQuad);
    }

  
    public virtual Tween FlyAnim()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_pawnModel.DOLocalMoveY(_pawnModel.position.y + _offScreenHeight, _duration).SetEase(Ease.InExpo));
        return sequence;
    }
    
    public virtual Tween DropDown()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            _pawnModel.localPosition = new Vector3(
                _pawnModel.localPosition.x,
                _offScreenHeight,
                _pawnModel.localPosition.z
            );
        });
        
        seq.Append(_pawnModel.DOLocalMoveY(0, _duration).SetEase(Ease.InSine));

        return seq;
    }
    
    public Tween WobbleAnim()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_pawnModel.DOPunchRotation(_punchRotation, _wobbleDuration, _vibrato, _elasticity).SetEase(Ease.OutQuad));

        return seq;
    }

    public Tween BounceAnim()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_pawnModel.DOPunchPosition(_punchPosition, _landDuration, _landVibrato).SetEase(Ease.OutQuad));

        return seq;
    }
    



    public Vector3 GetPosition() => _pawnModel.position;
    public Quaternion GetRotation() => _pawnModel.rotation;

    public void SetPosition(Vector3 pos)
    {
        if (_pawnModel != null) 
            _pawnModel.position = pos;
    }
    public void SetRotation(Quaternion rot)
    {
        if (_pawnModel != null) 
            _pawnModel.rotation = rot;
    }

    public void DoOffsetMove(Vector3 offset)
    {
        _pawnModel.DOLocalMove(offset, 0.3f).SetEase(Ease.OutQuad);
    }
    
    
}
