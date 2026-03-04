using System.Collections;
using Pawn;

namespace Interfaces
{
    public interface IEnemyBehavior
    {
        IEnumerator Execute(EnemyController enemy);
    }
}