using Pawn;

namespace Grid
{
    public interface INodeFeature
    {
        public void Initialize(Node owner);
        public void OnEnter(GridUnit unit);
    }
}