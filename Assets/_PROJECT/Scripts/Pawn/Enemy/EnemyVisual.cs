using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyVisual : GridUnitVisual
{
    public override IEnumerator DeadAnim(float duration, Action onComplete)
    {
        // 1. Ask the Graveyard Manager where this piece should land
        Vector3 finalRestingPlace = GraveyardManager.Instance.GetNextSlotPosition();
    
        // How high it needs to go to be off-screen (Adjust this based on your camera)
        float offScreenHeight = 15f; 

        Sequence deathSeq = DOTween.Sequence();

        // STEP 1: The Abduction (Fly straight up really fast)
        deathSeq.Append(_pawnModel.DOMoveY(_pawnModel.position.y + offScreenHeight, duration * 0.4f)
            .SetEase(Ease.InExpo)); // InExpo makes it start slow and ZOOM out

        // STEP 2: The Mid-Air Teleport & Tilt
        deathSeq.AppendCallback(() => 
        {
            // Move directly over the graveyard slot, keeping the height
            _pawnModel.position = new Vector3(finalRestingPlace.x, finalRestingPlace.y + offScreenHeight, finalRestingPlace.z);
        
            // Tilt the pawn 90 degrees so it looks "knocked over" before it drops
            _pawnModel.rotation = Quaternion.Euler(90f, UnityEngine.Random.Range(0f, 360f), 0f);
        });

        // STEP 3: The Drop (Slam into the table and bounce)
        deathSeq.Append(_pawnModel.DOMoveY(finalRestingPlace.y, duration * 0.4f)
            .SetEase(Ease.OutBounce)); // OutBounce makes it physically bounce on the table

        yield return deathSeq.WaitForCompletion();

        // IMPORTANT: Since the physical model is now in the graveyard, 
        // Instead, just disable their logic/colliders.
        onComplete?.Invoke();
    }
}
