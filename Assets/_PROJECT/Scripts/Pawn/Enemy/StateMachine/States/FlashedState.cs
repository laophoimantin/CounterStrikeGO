public class FlashedState : IEnemyState
{
	public void EnterState(EnemyController enemy)
	{
		enemy.EnemyVisual.ShowStunIcon();
		enemy.SetBehavior(enemy.FlashedBehavior);
	}

	public void ExecuteTurn(EnemyController enemy)
	{


	}

	public void ExitState(EnemyController enemy)
	{
		enemy.EnemyVisual.HideStunIcon();
	}
}