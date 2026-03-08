using DG.Tweening;
using UnityEngine;

public class ZoneVisual : GridUnitVisual
{

    void Start()
    {
        _pawnModel.gameObject.SetActive(false);
        _pawnModel.localPosition = new Vector3(0, _offScreenHeight, 0);
    }

    public Tween ActivateZoneModel()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => { _pawnModel.gameObject.SetActive(true); });

        seq.Append(DropDown());
        seq.Append(BounceAnim());
        return seq;
    }
}