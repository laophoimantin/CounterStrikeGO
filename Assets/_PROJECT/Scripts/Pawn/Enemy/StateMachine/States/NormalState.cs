public class NormalState : IEnemyState
{
	public void EnterState(EnemyController enemy)
	{
		enemy.SetBehavior(enemy.DefaultBehavior);
		enemy.CurrentBehavior.OnEnter(enemy);
	}

	public void ExecuteTurn(EnemyController enemy)
	{
	}

	public void ExitState(EnemyController enemy)
	{
		enemy.CurrentBehavior.OnExit(enemy);
	}
}
