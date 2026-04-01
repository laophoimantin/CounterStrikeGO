public class DistractedState : IEnemyState
{
	public void EnterState(EnemyController enemy)
	{
		enemy.EnemyVisual.ShowQuestionIcon();
		enemy.SetBehavior(enemy.FollowingNoiseBehavior);
	}

	public void ExecuteTurn(EnemyController enemy)
	{
		if (enemy.PathNavigator.HasReachedDestination)
		{
			enemy.ChangeState(enemy.StateNormal);
		}
	}

	public void ExitState(EnemyController enemy)
	{
		enemy.EnemyVisual.HideQuestionIcon();
	}
}