using System;
using System.Collections;
using System.Collections.Generic;

namespace Terrain.Mesh
{
    public class CornerBuffer : ICollection<Corner>
    {
        private readonly IDictionary<Corner, OrderedSetNode<Corner>> _dictionary;
        private readonly LinkedList<Corner> _linkedList;

        public CornerBuffer(): this(new CornerComparer())
        {
        }
        
        private CornerBuffer(IEqualityComparer<Corner> comparer)
        {
            _dictionary = new Dictionary<Corner, OrderedSetNode<Corner>>(comparer);
            _linkedList = new LinkedList<Corner>();
        }

        
        public DecomposedCornerBuffer Decompose()
        {
            var decomposed = new DecomposedCornerBuffer(_linkedList.Count);
            var index = 0;
            foreach (var item in _linkedList)
            {
                decomposed.Set(index, item);
                index++;
            }

            return decomposed;
        }

        public bool Remove(Corner corner)
        {
            throw new NotImplementedException();
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        void ICollection<Corner>.Add(Corner item)
        {
            Add(item);
        }

        public void Clear()
        {
            _linkedList.Clear();
            _dictionary.Clear();
        }

        public IEnumerator<Corner> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        public bool Contains(Corner item)
        {
            return item != null && _dictionary.ContainsKey(item);
        }

        public void CopyTo(Corner[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IEnumerable<Corner> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public bool TryGetIndexOf(Corner value, out int index)
        {
            index = 0;
            var found = _dictionary.TryGetValue(value, out var node);

            if (found)
                index = node.Index;

            return found;
        }

        public void Add(Corner item)
        {
            if (_dictionary.ContainsKey(item)) return;
            var node = _linkedList.AddLast(item);
            _dictionary.Add(item, new OrderedSetNode<Corner>(node, _linkedList.Count - 1));
        }
    }
}