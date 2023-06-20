using System;
using System.Collections.Generic;

namespace BlueFw.Utils;

public class ObjectPool<T> where T : class {

    /// <summary>
    /// Invoked when an object is retrieved from the pool.
    /// </summary>
    public event Action<T> ObjectRetrieved;

    /// <summary>
    /// Invoked when an object is returned to the pool.
    /// </summary>
    public event Action<T> ObjectReturned;

    readonly Stack<T> availableObjects;
    readonly Func<T> factory;

    /// <summary>
    /// Creates a new object pool of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="factoryMethod">A method for creating new <typeparamref name="T"/> objects.</param>
    /// <param name="initialPoolSize">(Optional) The number of objects to initialize the pool with.</param>
    /// <exception cref="ArgumentNullException"/>
    public ObjectPool(Func<T> factoryMethod, int initialPoolSize = 0) {
        factory = factoryMethod ?? throw new ArgumentNullException(nameof(factoryMethod));
        availableObjects = new Stack<T>(initialPoolSize);

        for (int i = 0; i < initialPoolSize; i++) {
            availableObjects.Push(factory());
        }
    }

    /// <summary>
    /// Retrieves the next available <typeparamref name="T"/>.
    /// </summary>
    public T Retrieve() {
        if (!availableObjects.TryPop(out T obj)) {
            obj = factory();
        }

        ObjectRetrieved?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Returns an object back to the pool.
    /// </summary>
    public void Return(T obj) {
        ObjectReturned?.Invoke(obj);
        availableObjects.Push(obj);
    }
}
