using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController _controller;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    public void AppendMoveSequence(Node targetNode, Sequence seq, float actionDuration)
    {
        IsMoving = true;

        Tween moveTween = _controller.PlayerVisual.MoveTo(targetNode.WorldPos, actionDuration);
        moveTween.OnComplete(() => { IsMoving = false; });
        seq.Append(moveTween);
        _controller.PlayerVisual.TryAddWobble(seq);
    }
}