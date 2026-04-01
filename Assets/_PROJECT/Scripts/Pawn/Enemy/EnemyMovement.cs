using DG.Tweening;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyController _controller;

    void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }
    
    public Sequence Move(Node targetNode, float actionDuration)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _controller.ChangeNode(targetNode); });
        seq.Append(_controller.EnemyVisual.MoveTo(targetNode.WorldPos, actionDuration));

        return seq;
    }

    public Sequence Rotate(Direction newDirection, float actionDuration)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { _controller.SetFacingDirection(newDirection); });
        Quaternion targetRot = GridMathUtility.GetRotation(newDirection);
        seq.Append(_controller.EnemyVisual.RotateTo(targetRot, actionDuration));
        return seq;
    }

}
