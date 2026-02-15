using System;
using Grid;
using UnityEngine;

public class DeployZoneUtility : UtilityController
{
    [Header("Zone Settings")]
    [SerializeField] private NodeZone _zonePrefab;
    [SerializeField] private int _duration = 3;

    protected override void OnLanded(Node targetNode, Action onComplete)
    {
        NodeZone newZone = Instantiate(_zonePrefab, targetNode.WorldPos, Quaternion.identity);
        newZone.Initialize(targetNode, _duration, onComplete);
        Die();
    }
}