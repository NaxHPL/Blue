using System.Collections.Generic;

namespace BlueFw.Utils;

public class ListPool<T> : CollectionPool<List<T>, T> { private ListPool() { } }

public class SetPool<T> : CollectionPool<HashSet<T>, T> { private SetPool() { } }

public class DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> { private DictionaryPool() { } }

public class CollectionPool<T1, T2> where T1 : ICollection<T2>, new() {

    static readonly Stack<T1> available = new Stack<T1>();

    protected CollectionPool() { }

    /// <summary>
    /// Gets a <typeparamref name="T1"/> from the pool.
    /// </summary>
    public static T1 Get() {
        return available.TryPop(out T1 collection) ? collection : new T1();
    }

    /// <summary>
    /// Returns a <typeparamref name="T1"/> back to the pool.
    /// </summary>
    public static void Return(T1 collection) {
        collection.Clear();
        available.Push(collection);
    }
}