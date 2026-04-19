using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stand still and do absolutely nothing
/// </summary>
[CreateAssetMenu(fileName = "Flashed", menuName = "Behav/Flashed", order = 11)]
public class FlashedBehavior : StandardEnemyBehavior
{
	public override List<BaseEnemyAction> PlanActions(EnemyController enemy)
	{
		var plan = new List<BaseEnemyAction>
		{
			new WaitAction(0.01f)
		};
		return plan;
	}
	protected override void CustomActions(List<BaseEnemyAction> baseList, EnemyController enemy)
	{
	}
}