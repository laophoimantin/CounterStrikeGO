using Pawn;

namespace Grid
{
    public interface INodeEffect
    {
        public void Initialize(Node owner);
        public void OnEnter(GridUnit unit);
    }
}