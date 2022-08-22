using System;
using System.Collections;
using System.Collections.Generic;

public sealed class OrderedSet<T> : ICollection<T>
{
    private readonly IDictionary<T, OrderedSetNode> _dictionary;
    private readonly LinkedList<T> _linkedList;

    public OrderedSet()
        : this(EqualityComparer<T>.Default)
    {
    }

    public OrderedSet(IEqualityComparer<T> comparer)
    {
        _dictionary = new Dictionary<T, OrderedSetNode>(comparer);
        _linkedList = new LinkedList<T>();
    }


    public bool Remove(T item)
    {
        if (item == null) return false;
        var found = _dictionary.TryGetValue(item, out var node);
        if (!found) return false;
        _dictionary.Remove(item);
        _linkedList.Remove(node.LinkedListNode);
        RecalculateIndexes();
        return true;
    }

    public int Count => _dictionary.Count;

    public bool IsReadOnly => _dictionary.IsReadOnly;

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    public void Clear()
    {
        _linkedList.Clear();
        _dictionary.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _linkedList.GetEnumerator();
    }

    public bool Contains(T item)
    {
        return item != null && _dictionary.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
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

    public void Add(IEnumerable<T> items)
    {
        foreach (var item in items) Add(item);
    }

    public int IndexOf(T key)
    {
        return _dictionary[key].Index;
    }

    private void Add(T item)
    {
        if (_dictionary.ContainsKey(item)) return;
        var node = _linkedList.AddLast(item);
        _dictionary.Add(item, new OrderedSetNode(node, _linkedList.Count - 1));
    }

    private class OrderedSetNode
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