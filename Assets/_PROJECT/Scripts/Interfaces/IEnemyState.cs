public interface IEnemyState
{
	void EnterState(EnemyController enemy);
	void ExecuteTurn(EnemyController enemy);
	void ExitState(EnemyController enemy);
}