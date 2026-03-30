using DG.Tweening;
using UnityEngine;

public class BaseZoneVisual : GridUnitVisual
{
    void Start()
    {
        _baseModel.gameObject.SetActive(false);
        _baseModel.localPosition = new Vector3(0, _offScreenHeight, 0);
    }

    public Tween ActivateZoneModel()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => { _baseModel.gameObject.SetActive(true); });

        seq.Append(DropDown());
        seq.Append(Bounce());
        return seq;
    }
}