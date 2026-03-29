using System;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class GridUnitVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform _baseModel;

    [Header("Spawn / Despawn")]
    [SerializeField] protected float _offScreenHeight = 15f;
    [SerializeField] protected float _flyDuration = 1f;

    [Header("Wobble")]
    [SerializeField] private float _wobbleAngle = 0.5f;
    [SerializeField] private float _wobbleDuration = 0.5f;
    [SerializeField] private int _wobbleVibrato = 10;
    [SerializeField] private float _wobbleElasticity = 1f;

    [Header("Bounce")]
    [SerializeField] private Vector3 _bounceOffset = new(0, 0.2f, 0);
    [SerializeField] private float _bounceDuration = 0.3f;
    [SerializeField] private int _bounceVibrato = 8;

    // Movement ======================================
    public Tween MoveTo(Vector3 targetPos, float duration)
    {
        // Move the whole transform
        return transform.DOMove(targetPos, duration).SetEase(Ease.Linear);
    }

    public Tween RotateTo(Quaternion targetRot, float duration)
    {
        return _baseModel.DORotateQuaternion(targetRot, duration).SetEase(Ease.OutQuad);
    }

    public void MoveOffset(Vector3 offset)
    {
        _baseModel.DOLocalMove(offset, 0.3f).SetEase(Ease.OutQuad);
    }

    // Feedback Animations ================================
    public Tween FlyUp()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_baseModel.DOLocalMoveY(_baseModel.position.y + _offScreenHeight, _flyDuration).SetEase(Ease.InSine));
        return sequence;
    }

    public Tween DropDown()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => { _baseModel.localPosition = new Vector3(_baseModel.localPosition.x, _offScreenHeight, _baseModel.localPosition.z); });

        seq.Append(_baseModel.DOLocalMoveY(0, _flyDuration).SetEase(Ease.InSine));
        return seq;
    }

    public void TryAddWobble(Sequence seq)
    {
        if (UnityEngine.Random.value < 0.5f)
            seq.Join(Wobble());
    }

    public Tween Wobble()
    {
        bool wobbleOnX = UnityEngine.Random.value > 0.8f;
        Vector3 randomPunch = wobbleOnX ? new Vector3(_wobbleAngle, 0, 0) : new Vector3(0, 0, _wobbleAngle);
        return _baseModel.DOPunchRotation(randomPunch, _wobbleDuration, _wobbleVibrato, _wobbleElasticity).SetEase(Ease.OutQuad);
    }

    public Tween Bounce()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(_baseModel.DOPunchPosition(_bounceOffset, _bounceDuration, _bounceVibrato).SetEase(Ease.OutQuad));

        return seq;
    }

    // Direct Transform ===================================
    public Quaternion GetRotation() => _baseModel.rotation;

    public void SetPosition(Vector3 pos)
    {
        if (_baseModel != null)
            _baseModel.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        if (_baseModel != null)
            _baseModel.rotation = rot;
    }
}