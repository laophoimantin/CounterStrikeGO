using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GridUnitVisual : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected Transform _pawnModel;

    protected virtual void Awake()
    {
        if (_pawnModel == null) 
            _pawnModel = transform.GetChild(0);
    }

    public virtual IEnumerator MoveTo(Vector3 targetPos, float duration)
    {
        yield return transform.DOMove(targetPos, duration)
            .SetEase(Ease.Linear)
            .WaitForCompletion();
    }

    public virtual IEnumerator RotateTo(Quaternion targetRot, float duration)
    {
        yield return _pawnModel.DORotateQuaternion(targetRot, duration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();
    }

    public Tween RotateToTween(Quaternion targetRot, float duration)
    {
       return _pawnModel.DORotateQuaternion(targetRot, duration)
            .SetEase(Ease.OutQuad);
    }

    // Test
    public virtual Sequence DeadAnim(float duration)
    {
        Sequence deathSeq = DOTween.Sequence();
        deathSeq.Append(_pawnModel.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
        return deathSeq;
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
