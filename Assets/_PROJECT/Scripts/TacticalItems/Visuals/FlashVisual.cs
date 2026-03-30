using DG.Tweening;
using UnityEngine;

public class FlashVisual : UtilityVisual
{
    public override Sequence GetThrowSequence(Vector3 targetPos)
    {
        Vector3 target = new Vector3(targetPos.x, targetPos.y + 3f, targetPos.z);
        return base.GetThrowSequence(target);
    }

    public override Sequence GetLandedAnim()
    {
        return null;
    }
}