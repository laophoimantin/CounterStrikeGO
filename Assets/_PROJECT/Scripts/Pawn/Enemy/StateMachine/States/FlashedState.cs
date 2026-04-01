using UnityEngine;

public class FlashedState : IEnemyState
{
	private int _flashTurnsRemaining;
	
	public void EnterState(EnemyController enemy)
	{
		enemy.EnemyVisual.ShowStunIcon();
		enemy.SetBehavior(enemy.FlashedBehavior);
	}

	public void ExecuteTurn(EnemyController enemy)
	{
		if (_flashTurnsRemaining > 0)
		{
			_flashTurnsRemaining--;
		}

		if (_flashTurnsRemaining <= 0)
		{
			if (!enemy.PathNavigator.HasReachedDestination)
			{
				enemy.ChangeState(enemy.StateDistracted);
			}
			else
			{
				enemy.ChangeState(enemy.StateNormal);
			}
		}
	}

	public void ExitState(EnemyController enemy)
	{
		enemy.EnemyVisual.HideStunIcon();
		_flashTurnsRemaining = 0;
	}
	
	public void AddFlashDuration(int duration)
	{
		_flashTurnsRemaining = Mathf.Max(_flashTurnsRemaining, duration);
	}
	public bool IsFinished()
	{
		return _flashTurnsRemaining <= 0;
	}
}