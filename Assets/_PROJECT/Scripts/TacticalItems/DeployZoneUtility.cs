using DG.Tweening;
using UnityEngine;

public class DeployZoneUtility : UtilityController
{
    [Header("Zone Settings")]
    [SerializeField] private BaseZone _baseZonePrefab;
    [SerializeField] private int _duration = 3;

    protected override Tween GetOnLandedSequence(Node targetNode, Team team)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(SpawnZone(targetNode));
        return seq;
    }

    private Tween SpawnZone(Node node)
    {
        BaseZone baseZone = Instantiate(_baseZonePrefab, node.WorldPos, Quaternion.identity);
        return baseZone.Initialize(node, _duration);
    }
}