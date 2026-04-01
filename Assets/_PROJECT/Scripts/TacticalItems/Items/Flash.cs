using DG.Tweening;
using UnityEngine;

public class Flash : UtilityController
{
    [SerializeField] private int _flashAmount = 2;

    protected override Tween GetOnLandedSequence(Node targetNode, Team team)
    {
        var nodes = NodeManager.Instance.GetNodesInRange(targetNode, 1, true);

        Sequence seq = DOTween.Sequence();
        bool hasReaction = false;

        foreach (var node in nodes)
        {
            foreach (GridOccupant occupant in node.GetAllOccupants())
            {
                IFlashable victimWithEyes = occupant.GetComponent<IFlashable>();
                
                if (victimWithEyes != null)
                {
                    Tween reaction = victimWithEyes.GetFlashed(_flashAmount);

                    if (reaction != null) 
                    {
                        seq.Insert(0, reaction);
                        hasReaction = true;
                    }
                }
            }
        }

        return hasReaction ? seq : null;
    }
}