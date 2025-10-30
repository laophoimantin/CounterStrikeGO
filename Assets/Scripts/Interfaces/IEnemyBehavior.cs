using System.Collections;
using Characters.Enemy;

namespace Interfaces
{
    public interface IEnemyBehavior
    {
        IEnumerator Execute(EnemyController enemy);
    }
}