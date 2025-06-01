using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// A simple wrapper around <see cref="ConcurrentDictionary(T, Empty)"/> which
/// functions similarly to a hash set.
/// </summary>
public class ConcurrentSet<T> : ICollection<T>
{
    private struct Empty { }

    private readonly ConcurrentDictionary<T, Empty> values = new();

    public bool Add(T item)
        => values.TryAdd(item, default);

    public bool Remove(T item)
        => values.Remove(item, out _);

    public bool Contains(T item)
        => values.ContainsKey(item);

    public void Clear()
        => values.Clear();

    void ICollection<T>.Add(T item)
        => (values as ICollection<T>).Add(item);

    public void CopyTo(T[] array, int arrayIndex)
        => (values as ICollection<T>).CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator()
        => values.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => values.Keys.GetEnumerator();

    bool ICollection<T>.IsReadOnly => false;

    public int Count => values.Count;
    public bool IsEmpty => values.IsEmpty;
}
