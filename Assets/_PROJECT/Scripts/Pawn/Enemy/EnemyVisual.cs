using DG.Tweening;
using UnityEngine;

public class EnemyVisual : GridUnitVisual
{
    [SerializeField] private float _offScreenHeight = 15f;

    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _markDuration = 1f;
    
    [Header("Marks")]
    [SerializeField] private Transform _questionMark;
    [SerializeField] private Transform _stunMark;

    void Start()
    {
        _questionMark.gameObject.SetActive(false);
        _stunMark.gameObject.SetActive(false);
    }

    public override Sequence DeadAnim()
    {
        Sequence deathSeq = DOTween.Sequence();

        deathSeq.Append(_pawnModel.DOMoveY(_pawnModel.position.y + _offScreenHeight, _duration).SetEase(Ease.InExpo));

        return deathSeq;
    }

    public Sequence FlydownAnim()
    {
        Sequence flydownSeq = DOTween.Sequence();
        
        Vector3 finalRestingPlace = GraveyardManager.Instance.GetNextSlotPosition();
        flydownSeq.AppendCallback(() => { _pawnModel.position = new Vector3(finalRestingPlace.x, finalRestingPlace.y + _offScreenHeight, finalRestingPlace.z); });
        flydownSeq.Append(_pawnModel.DOMoveY(finalRestingPlace.y, _duration)
            .SetEase(Ease.OutCubic));
        
        return flydownSeq;
    }

    public Sequence QuestionMarkAnim()
    {
        Sequence questionSeq = DOTween.Sequence();
        questionSeq.AppendCallback(() => { _questionMark.gameObject.SetActive(true); });

        questionSeq.Append(_questionMark.DOLocalMoveY(-0.5f, _markDuration).SetRelative().SetEase(Ease.OutBounce));
        questionSeq.AppendCallback(() => { _questionMark.gameObject.SetActive(false); });
        return questionSeq;
    }

  

    public Sequence StunMarkAnim()
    {
        Sequence stunSequence = DOTween.Sequence();
        stunSequence.AppendCallback(() => { _stunMark.gameObject.SetActive(true); });
        stunSequence.Append(_stunMark.DOLocalMoveY(1f, _markDuration).SetRelative().SetEase(Ease.Linear));

        return stunSequence;
    }

    public void HideStunMark()
    {
        _stunMark.gameObject.SetActive(false);
    }
}