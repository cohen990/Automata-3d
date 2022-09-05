using System.Collections.Generic;

namespace Terrain
{
    public class OrderedSetNode<T>
    {
        public readonly int Index;
        public readonly LinkedListNode<T> LinkedListNode;

        public OrderedSetNode(LinkedListNode<T> linkedListNode, int index)
        {
            Index = index;
            LinkedListNode = linkedListNode;
        }
    }
}