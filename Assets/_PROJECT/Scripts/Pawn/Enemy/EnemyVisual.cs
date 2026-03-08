using DG.Tweening;
using UnityEngine;

public class EnemyVisual : GridUnitVisual
{
    [SerializeField] private float _markDuration = 1f;

    [Header("Marks")]
    [SerializeField] private Transform _questionMark;
    [SerializeField] private Transform _stunMark;



    void Start()
    {
        _questionMark.gameObject.SetActive(false);
        _stunMark.gameObject.SetActive(false);
    }

    public Tween DropToGraveyard()
    {
        Sequence seq = DOTween.Sequence();
        Vector3 finalRestingPlace = GraveyardManager.Instance.GetNextSlotPosition();

        seq.Append(DropDown());
        return seq;
    }
    
    public Tween QuestionMarkAnim()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _questionMark.gameObject.SetActive(true); });

        seq.Append(_questionMark.DOLocalMoveY(-0.5f, _markDuration).SetRelative().SetEase(Ease.OutBounce));
        seq.AppendCallback(() => { _questionMark.gameObject.SetActive(false); });
        return seq;
    }

    public Tween StunMarkAnim()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _stunMark.gameObject.SetActive(true); });
        seq.Append(_stunMark.DOLocalMoveY(1f, _markDuration).SetRelative().SetEase(Ease.Linear));

        return seq;
    }


    public void HideStunMark()
    {
        _stunMark.gameObject.SetActive(false);
    }
}