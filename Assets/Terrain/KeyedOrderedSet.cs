﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Terrain
{
    public class KeyedOrderedSet<TKey, TValue> : ICollection<TValue>
    {
        private readonly IDictionary<TKey, OrderedSetNode> _dictionary;
        private readonly LinkedList<TValue> _linkedList;

        private readonly Func<TValue, TKey> _valueToKey;

        protected KeyedOrderedSet(Func<TValue, TKey> valueToKey)
            : this(EqualityComparer<TKey>.Default)
        {
            _valueToKey = valueToKey;
        }

        private KeyedOrderedSet(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new Dictionary<TKey, OrderedSetNode>(comparer);
            _linkedList = new LinkedList<TValue>();
        }


        public bool Remove(TValue key)
        {
            if (key == null) return false;
            var found = _dictionary.TryGetValue(_valueToKey(key), out var node);
            if (!found) return false;
            _dictionary.Remove(_valueToKey(key));
            _linkedList.Remove(node.LinkedListNode);
            RecalculateIndexes();
            return true;
        }

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        void ICollection<TValue>.Add(TValue item)
        {
            Add(item);
        }

        public void Clear()
        {
            _linkedList.Clear();
            _dictionary.Clear();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        public bool Contains(TValue item)
        {
            return item != null && _dictionary.ContainsKey(_valueToKey(item));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void RecalculateIndexes()
        {
            throw new NotImplementedException();
        }

        public void Add(IEnumerable<TValue> items)
        {
            foreach (var item in items) 
                Add(item);
        }
        
        public void TryGetIndexOf(TValue value, out int index)
        {
            index = 0;
            var found = _dictionary.TryGetValue(_valueToKey(value), out var node);
            
            if(found)
                index = node.Index;
        }

        private void Add(TValue item)
        {
            if (_dictionary.ContainsKey(_valueToKey(item))) return;
            var node = _linkedList.AddLast(item);
            _dictionary.Add(_valueToKey(item), new OrderedSetNode(node, _linkedList.Count - 1));
        }

        private class OrderedSetNode
        {
            public readonly int Index;
            public readonly LinkedListNode<TValue> LinkedListNode;

            public OrderedSetNode(LinkedListNode<TValue> linkedListNode, int index)
            {
                Index = index;
                LinkedListNode = linkedListNode;
            }
        }
    }
}