using DG.Tweening;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyController _controller;

    void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }
    
    public Tween Move(Node targetNode, float actionDuration)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _controller.ChangeNode(targetNode); });
        seq.Append(_controller.EnemyVisual.MoveTo(targetNode.WorldPos, actionDuration));

        return seq;
    }

    public Tween Rotate(Direction newDirection, float actionDuration)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _controller.SetFacingDirection(newDirection); });
        Quaternion targetRot = GridMathUtility.GetRotation(newDirection);
        seq.Append(_controller.EnemyVisual.RotateTo(targetRot, actionDuration));
        return seq;
    }
    
    
    public Tween GetBurnEscapeSeq(Node targetNode, Direction targetDir, float actionDuration)
    {
        Sequence seq = DOTween.Sequence();

        if (_controller.CurrentFacingDirection != targetDir)
        {
            seq.Append(Rotate(targetDir, actionDuration));
        }
        seq.Append(Move(targetNode, actionDuration));

        return seq;
    }
}
