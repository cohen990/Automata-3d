namespace Terrain.Mesh
{
    public class CornerBuffer : KeyedOrderedSet<Corner.CornerBufferKey, Corner>
    {
        public CornerBuffer() : base(x => x.ToCornerBufferKey())
        {
        }
    }
}