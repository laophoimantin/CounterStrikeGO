using System;
using System.Collections;
using DG.Tweening;
using Grid;
using UnityEngine;

public class DeployZoneUtility : UtilityController
{
    [Header("Zone Settings")]
    [SerializeField] private NodeZone _zonePrefab;
    [SerializeField] private int _duration = 3;

    // protected override IEnumerator OnLanded(Node targetNode)
    // {
    //     NodeZone newZone = Instantiate(_zonePrefab, targetNode.WorldPos, Quaternion.identity);
    //     yield return newZone.Initialize(targetNode, _duration);
    // }


    protected override Sequence GetOnLandedSequence(Node targetNode)
    {
        return null;
    }
}