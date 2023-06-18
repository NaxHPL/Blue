using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueFw;

/// <summary>
/// A basic wrapper around an array that automatically expands when it reached capacity.
/// Provides direct access to the buffer for fast retrieval.
/// </summary>
public class FastList<T> : IEnumerable<T> {

    const int DEFAULT_CAPACITY = 4;

    /// <summary>
    /// Direct access to the item buffer array.
    /// </summary>
    public T[] Buffer;

    /// <summary>
    /// Direct access to the length of the filled items in the buffer. DO NOT MODIFY.
    /// </summary>
    public int Length;

    /// <summary>
    /// Gets/sets the item at the specified index.
    /// </summary>
    /// <remarks>For slightly better performace, access the item buffer directly using <see cref="Length"/> for the total item count.</remarks>
    public T this[int index] {
        get {
            if (index < 0 || index >= Length) {
                throw new IndexOutOfRangeException();
            }

            return Buffer[index];
        }
        set {
            if (index < 0 || index >= Length) {
                throw new IndexOutOfRangeException();
            }

            Buffer[index] = value;
        }
    }

    readonly bool isValueType;

    /// <summary>
    /// Creates a new <see cref="FastList{T}"/>.
    /// </summary>
    public FastList() : this(DEFAULT_CAPACITY) { }

    /// <summary>
    /// Creates a new <see cref="FastList{T}"/> with an initial capacity of <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">The initial size of the buffer array.</param>
    public FastList(int capacity) {
        Buffer = new T[capacity];
        isValueType = typeof(T).IsValueType;
    }

    /// <summary>
    /// Creates a new <see cref="FastList{T}"/> and copies the contents of <paramref name="enumerable"/> into it.
    /// </summary>
    /// <param name="enumerable"></param>
    public FastList(IEnumerable<T> enumerable) {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));

        if (enumerable is ICollection<T> c) {
            int count = c.Count;
            if (count == 0) {
                Buffer = new T[DEFAULT_CAPACITY];
            }
            else {
                Buffer = new T[count];
                c.CopyTo(Buffer, 0);
                Length = count;
            }
        }
        else {
            Buffer = new T[DEFAULT_CAPACITY];
            using IEnumerator<T> en = enumerable.GetEnumerator();
            while (en.MoveNext()) {
                Add(en.Current);
            }
        }

        isValueType = typeof(T).IsValueType;
    }

    /// <summary>
    /// Appends an item to the end of the list.
    /// </summary>
    public void Add(T item) {
        if (Length == Buffer.Length) {
            EnsureCapacity(Length + 1);
        }

        Buffer[Length++] = item;
    }

    /// <summary>
    /// Adds all items from <paramref name="enumerable"/>.
    /// </summary>
    public void AddRange(IEnumerable<T> enumerable) {
        ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));

        if (enumerable == this || enumerable == Buffer) {
            int LengthX2 = Length * 2;
            EnsureCapacity(LengthX2);
            Array.Copy(Buffer, 0, Buffer, Length, Length);
            Length = LengthX2;
        }
        else {
            using IEnumerator<T> en = enumerable.GetEnumerator();
            while (en.MoveNext()) {
                Add(en.Current);
            }
        }
    }

    /// <summary>
    /// Ensures the item buffer has at least <paramref name="minimum"/> capacity.
    /// </summary>
    public void EnsureCapacity(int minimum) {
        if (Buffer.Length >= minimum) {
            return;
        }

        int newCapacity = MathHelper.Max(Buffer.Length * 2, DEFAULT_CAPACITY);
        Array.Resize(ref Buffer, newCapacity);
    }

    /// <summary>
    /// Adds an item at the specified index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public void Insert(T item, int index) {
        if (index < 0 || index > Length) {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Expected a value in the range [0,{Length}]");
        }

        if (Length == Buffer.Length) {
            EnsureCapacity(Length + 1);
        }

        if (index < Length) {
            Array.Copy(Buffer, index, Buffer, index + 1, Length - index);
        }

        Buffer[index] = item;
        Length++;
    }

    /// <summary>
    /// Removes an item.
    /// </summary>
    /// <returns><see langword="true"/> if <paramref name="item"/> was found and removed; otherwise <see langword="false"/>.</returns>
    public bool Remove(T item) {
        int index = Array.IndexOf(Buffer, item, 0, Length);

        if (index < 0) {
            return false;
        }

        RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public void RemoveAt(int index) {
        if (index < 0 || index >= Length) {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Expected a value in the range [0,{Length-1}]");
        }

        Length--;

        Array.Copy(Buffer, index + 1, Buffer, index, Length - index);

        if (!isValueType) {
            Buffer[Length] = default;
        }
    }

    /// <summary>
    /// Returns the index of <paramref name="item"/> in the list. If <paramref name="item"/> is not found, returns -1.
    /// </summary>
    public int IndexOf(T item) {
        return Array.IndexOf(Buffer, item, 0, Length);
    }

    /// <summary>
    /// Clears all items in the list.
    /// </summary>
    public void Clear() {
        if (!isValueType) {
            Array.Clear(Buffer, 0, Length);
        }

        Length = 0;
    }

    /// <summary>
    /// Checks if <paramref name="item"/> exists in the list.
    /// </summary>
    public bool Contains(T item) {
        EqualityComparer<T> eqCompare = EqualityComparer<T>.Default;

        for (int i = 0; i < Length; i++) {
            if (eqCompare.Equals(Buffer[i], item)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Sorts the items in the list using the <see cref="IComparable{T}"/> generic implementation of each item.
    /// </summary>
    public void Sort() {
        Array.Sort(Buffer, 0, Length);
    }

    /// <summary>
    /// Sorts the items in the list using the specified <see cref="IComparer"/>.
    /// </summary>
    public void Sort(IComparer comparer) {
        Array.Sort(Buffer, 0, Length, comparer);
    }

    /// <summary>
    /// Sorts the items in the list using the specified <see cref="IComparer{T}"/>.
    /// </summary>
    public void Sort(IComparer<T> comparer) {
        Array.Sort(Buffer, 0, Length, comparer);
    }

    public IEnumerator<T> GetEnumerator() {
        return new FastListEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return new FastListEnumerator(this);
    }

    struct FastListEnumerator : IEnumerator<T> {

        public readonly T Current => list.Buffer[index];
        readonly object IEnumerator.Current => list.Buffer[index];

        readonly FastList<T> list;
        int index;

        public FastListEnumerator(FastList<T> list) {
            this.list = list;
            index = -1;
        }

        public bool MoveNext() {
            return ++index < list.Length;
        }

        public readonly void Dispose() { }

        public readonly void Reset() { }
    }
}
