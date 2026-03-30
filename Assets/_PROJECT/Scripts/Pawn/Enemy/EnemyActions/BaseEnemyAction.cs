using System.Collections;

public abstract class BaseEnemyAction
{
    public abstract IEnumerator Execute(EnemyController enemy);
}