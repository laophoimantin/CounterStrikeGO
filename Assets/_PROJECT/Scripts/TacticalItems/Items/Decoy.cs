using DG.Tweening;

public class Decoy : UtilityController
{
    protected override Tween GetOnLandedSequence(Node targetNode, Team team)
    {
        var nodes = NodeManager.Instance.GetNodesInRange(targetNode, 1, true);

        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;

        foreach (var node in nodes)
        {
            foreach (GridOccupant occupant in node.GetAllOccupants())
            {
                INoiseListener listener = occupant.GetComponent<INoiseListener>();
                if (listener != null)
                {
                    Tween reaction = listener.HearNoise(targetNode);
                    if (reaction != null)
                    {
                        seq.Insert(0, reaction);
                        hasReaction = true;
                    }
                }
            }
            // foreach (var enemy in node.GetUnitsByType<EnemyController>())
            // {
            //     if (enemy.IsFlashed) continue;
            //
            //     var reaction = enemy.HearNoise(targetNode);
            //
            //     if (reaction == null) continue;
            //
            //     seq.Insert(0, reaction);
            //     hasReaction = true;
            // }
        }

        return hasReaction ? seq : null;
    }
}