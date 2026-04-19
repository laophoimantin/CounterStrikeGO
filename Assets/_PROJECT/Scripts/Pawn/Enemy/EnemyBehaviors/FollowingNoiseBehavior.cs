using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When an enemy is distracted by the decoy, they will try to move to where the decoy landed
/// </summary>
[CreateAssetMenu(fileName = "Noise", menuName = "Behav/Noise", order = 10)]
public class FollowingNoiseBehavior : StandardEnemyBehavior
{
    protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
    {
        var navigator = enemy.PathNavigator;
        var nextNode = navigator.NextNode;
        if (nextNode == null)
        {
            return;
        }

        baseList.Add(new MoveAction(nextNode));

        var upcoming = navigator.UpcomingNode;
        if (upcoming != null)
        {
            Direction dir = GridMathUtility.GetDirectionFromTargetNode(nextNode, upcoming);
            baseList.Add(new RotateAction(dir));
        }
    }
}

//public abstract class BaseEnemyState
//{
//    protected EnemyController controller;
//    public BaseEnemyState(EnemyController controller)
//    {
//        this.controller = controller;
//    }

//    public virtual void OnEnter() { }
//	public virtual void OnUpdate() { }
//	public virtual void OnExit() { }
//}

//public class EnemyStunState : BaseEnemyState
//{
//    public EnemyStunState(EnemyController controller) : base(controller)
//    {
//    }

//    public override void OnExit()
//    {
//        controller.EnemyVisual.HideStunIcon();
//	}
//}