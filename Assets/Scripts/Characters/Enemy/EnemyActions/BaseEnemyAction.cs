using System.Collections;

namespace Characters.Enemy.EnemyActions
{
    public abstract class BaseEnemyAction
    {
        public abstract IEnumerator Execute(EnemyController enemy);
    }
}