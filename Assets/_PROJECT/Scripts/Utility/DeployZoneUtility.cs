using System;
using System.Collections;
using DG.Tweening;
using Grid;
using UnityEngine;

public class DeployZoneUtility : UtilityController
{
    [Header("Zone Settings")]
    [SerializeField] private Zone _zonePrefab;
    [SerializeField] private int _duration = 3;

    protected override Tween GetOnLandedSequence(Node targetNode)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(SpawnZone(targetNode));
        return seq;
    }
    private Tween SpawnZone(Node node)
    {
        Zone zone = Instantiate(_zonePrefab, node.WorldPos, Quaternion.identity);
        return zone.Initialize(node, _duration);
    }
}