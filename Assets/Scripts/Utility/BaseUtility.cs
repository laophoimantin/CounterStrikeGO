using DG.Tweening;
using Grid;
using Pawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUtility : MonoBehaviour
{

    private PlayerController _owner;

    void Start()
    {
        
    }
    public void OnPickUp(PlayerController player)
    {
        _owner = player;

        var col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // _owner.Switch Throwing Model

        transform.SetParent(_owner.transform, false);
        transform.position = _owner.transform.position + Vector3.up * 2f;
    }

    public void Throw(Node targetNode)
    {

        transform.DOJump(targetNode.WorldPos, 1.5f, 1, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                OnLanded(targetNode);
                Destroy(gameObject, 1f);
            });
    }

    public abstract void OnLanded(Node targetNode);
}
