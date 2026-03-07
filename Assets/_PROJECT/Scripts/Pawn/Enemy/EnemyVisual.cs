using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyVisual : GridUnitVisual
{
    [SerializeField] private Transform _questionMark;

    void Start()
    {
        _questionMark.gameObject.SetActive(false);
    }

    public override Sequence DeadAnim(float duration)
    {
        Vector3 finalRestingPlace = GraveyardManager.Instance.GetNextSlotPosition();
        float offScreenHeight = 15f; 

        Sequence deathSeq = DOTween.Sequence();

        deathSeq.Append(_pawnModel.DOMoveY(_pawnModel.position.y + offScreenHeight, duration * 0.4f)
            .SetEase(Ease.InExpo)); 

        deathSeq.AppendCallback(() => 
        {
            _pawnModel.position = new Vector3(finalRestingPlace.x, finalRestingPlace.y + offScreenHeight, finalRestingPlace.z);
        });

        // STEP 3: The Drop (Slam into the table and bounce)
        deathSeq.Append(_pawnModel.DOMoveY(finalRestingPlace.y, duration * 0.4f)
            .SetEase(Ease.OutBounce)); 

        return deathSeq;
    }

    public Sequence QuestionMarkAnim()
    {
        Sequence questionSeq = DOTween.Sequence();
        questionSeq.AppendCallback(() =>
        {
            _questionMark.gameObject.SetActive(true);
        });

        questionSeq.Append(_questionMark.DOLocalMoveY(-0.5f, 1.5f).SetRelative().SetEase(Ease.OutBounce));
        questionSeq.AppendCallback(() =>
        {
            _questionMark.gameObject.SetActive(false);
        });
        return questionSeq;
    }
}
