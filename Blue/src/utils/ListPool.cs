using System.Collections.Generic;

namespace BlueFw;

/// <summary>
/// A simple, static object pool for easy list reuse.
/// </summary>
public static class ListPool<T> {

    static readonly Stack<List<T>> available = new Stack<List<T>>();

    /// <summary>
    /// Gets a list of type <typeparamref name="T"/>
    /// </summary>
    public static List<T> Get() {
        return available.TryPop(out List<T> list) ? list : new List<T>();
    }

    /// <summary>
    /// Returns a list back to the pool.
    /// </summary>
    public static void Return(List<T> list) {
        list.Clear();
        available.Push(list);
    }
}
