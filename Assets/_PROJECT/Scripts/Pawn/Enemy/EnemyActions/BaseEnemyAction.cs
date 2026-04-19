using System.Collections;

/// <summary>
/// The things that enemy will do in their turn
/// </summary>
public abstract class BaseEnemyAction
{
    public abstract IEnumerator Execute(EnemyController enemy);
}