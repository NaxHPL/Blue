using System.Collections.Concurrent;

namespace BlueFw.Utils;

/// <summary>
/// A simple static, generic object pool.
/// </summary>
public static class Pool<T> where T : class, new() {

    static readonly ConcurrentStack<T> available = new ConcurrentStack<T>();

    /// <summary>
    /// Gets the next available <typeparamref name="T"/>.
    /// </summary>
    public static T Get() {
        return available.TryPop(out T obj) ? obj : new T();
    }

    /// <summary>
    /// Returns an object back to the pool.
    /// </summary>
    public static void Return(T obj) {
        available.Push(obj);
    }
}
