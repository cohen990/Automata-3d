namespace Terrain
{
    public sealed class OrderedSet<T> : KeyedOrderedSet<T, T>
    {
        public OrderedSet() : base(x => x)
        {
        }
    }
}