public class NormalState : IEnemyState
{
	public void EnterState(EnemyController enemy)
	{
		enemy.SetBehavior(enemy.DefaultBehavior);
	}

	public void ExecuteTurn(EnemyController enemy)
	{
	}

	public void ExitState(EnemyController enemy)
	{
	}
}
