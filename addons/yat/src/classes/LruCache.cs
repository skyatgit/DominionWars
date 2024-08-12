using System;
using System.Collections.Generic;

namespace YAT.Classes;

public class LruCache<TKey, TValue> where TKey : notnull where TValue : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<LruItem<TKey, TValue>>> _cache;
    private readonly ushort _capacity;
    private readonly LinkedList<LruItem<TKey, TValue>> _lruList;

    public LruCache(ushort capacity)
    {
        if (capacity == 0)
        {
            throw new ArgumentException("Capacity must be greater than 0.", nameof(capacity));
        }

        _capacity = capacity;
        _cache = new Dictionary<TKey, LinkedListNode<LruItem<TKey, TValue>>>(capacity);
        _lruList = new LinkedList<LruItem<TKey, TValue>>();
    }

    public int Size => _cache.Count;

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>
    ///     The value associated with the specified key, or the default value of <typeparamref name="TValue" /> if the key
    ///     is not found.
    /// </returns>
    public TValue Get(TKey key)
    {
        if (_cache.TryGetValue(key, out LinkedListNode<LruItem<TKey, TValue>> node))
        {
            // Move the accessed item to the front of the list
            _lruList.Remove(node);
            _lruList.AddFirst(node);
            return node.Value.Value;
        }

        return default!;
    }

    /// <summary>
    ///     Adds a new key-value pair to the cache.
    ///     If the cache is already at capacity, the least recently used item is removed.
    /// </summary>
    /// <param name="key">The key of the item to add.</param>
    /// <param name="value">The value of the item to add.</param>
    public void Add(TKey key, TValue value)
    {
        if (_cache.Count >= _capacity && _lruList.Last is not null)
        {
            // Remove the least recently used item
            LinkedListNode<LruItem<TKey, TValue>> lastNode = _lruList.Last;
            _lruList.RemoveLast();
            _cache.Remove(lastNode!.Value.Key);
        }

        _lruList.AddFirst(new LinkedListNode<LruItem<TKey, TValue>>(new LruItem<TKey, TValue>(key, value)));
        _cache[key] = _lruList.First!;
    }
}

public class LruItem<TKey, TValue> where TKey : notnull where TValue : notnull
{
    public LruItem(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public TKey Key { get; }
    public TValue Value { get; }
}
