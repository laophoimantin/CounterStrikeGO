public class DistractedState : IEnemyState
{
	public void EnterState(EnemyController enemy)
	{
		UnityEngine.Debug.Log("nfdasnjdf");
		enemy.EnemyVisual.ShowQuestionIcon();
		enemy.SetBehavior(enemy.FollowingNoiseBehavior);
	}

	public void ExecuteTurn(EnemyController enemy)
	{


	}

	public void ExitState(EnemyController enemy)
	{
		enemy.EnemyVisual.HideQuestionIcon();
	}
}