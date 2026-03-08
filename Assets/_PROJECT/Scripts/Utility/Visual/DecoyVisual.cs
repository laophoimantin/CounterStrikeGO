using DG.Tweening;
using UnityEngine;

public class DecoyVisual : UtilityVisual
{
    [Header("Decoy Effect")]
    [SerializeField] private Transform _decoyEffect;
    [SerializeField] private float _expandScale;
    [SerializeField] private float _expandDuration;

    public override void Start()
    {
        base.Start();
        _decoyEffect.localScale = Vector3.zero;
        _decoyEffect.gameObject.SetActive(false);
    }

    public override Sequence GetLandedAnim()
    {
        Sequence decoySeq = DOTween.Sequence();

        decoySeq.AppendCallback(() =>
        {
            _decoyEffect.gameObject.SetActive(true);
            _decoyEffect.position = _utilityModel.position + Vector3.up * 0.5f;
            _decoyEffect.localScale = Vector3.zero;
        });

        Vector3 endScale = new Vector3(_expandScale, _expandScale, 1f);
        decoySeq.Append(_decoyEffect.DOScale(endScale, _expandDuration).SetEase(Ease.OutCubic));
        decoySeq.AppendCallback(() => _decoyEffect.gameObject.SetActive(false));

        return decoySeq;
    }
}