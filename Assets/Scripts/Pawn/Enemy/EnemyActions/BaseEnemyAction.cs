using System.Collections;

namespace Pawn
{
    public abstract class BaseEnemyAction
    {
        public abstract IEnumerator Execute(EnemyController enemy);
    }
}