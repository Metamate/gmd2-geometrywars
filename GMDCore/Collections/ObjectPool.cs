using System;
using System.Collections.Generic;

namespace GMDCore.Collections;

// Generic stack-based object pool. Avoids per-frame heap allocations for
// frequently created/destroyed objects (e.g. pooled entities).
public sealed class ObjectPool<T> where T : class
{
    private readonly Stack<T> _pool = new();
    private readonly Func<T> _factory;

    public ObjectPool(Func<T> factory, int initialCapacity = 0)
    {
        _factory = factory;
        for (int i = 0; i < initialCapacity; i++)
            _pool.Push(factory());
    }

    public T Get() => _pool.Count > 0 ? _pool.Pop() : _factory();

    public void Return(T item) => _pool.Push(item);
}

