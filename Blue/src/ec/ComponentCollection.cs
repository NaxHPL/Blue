﻿using BlueFw.Utils;
using System;
using System.Collections.Generic;

namespace BlueFw;

internal class ComponentCollection {

    /// <summary>
    /// The number of components in this <see cref="ComponentCollection"/>.
    /// </summary>
    public int Count => components.Length;

    /// <summary>
    /// Gets the <see cref="Component"/> at the specified index.
    /// </summary>
    public Component this[int index] {
        get {
            if (index >= components.Length) {
                throw new IndexOutOfRangeException();
            }

            return components.Buffer[index];
        }
    }

    readonly FastList<Component> components = new FastList<Component>();
    readonly HashSet<uint> componentInstanceIds = new HashSet<uint>(); // Used for checking if the collection contains a component

    /// <summary>
    /// Adds a component to the collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="component"/> wasn't already in this collection and added successfully; otherwise <see langword="false"/>.
    /// </returns>
    public bool Add(Component component) {
        if (component == null) {
            return false;
        }

        if (!componentInstanceIds.Add(component.InstanceID)) {
            return false;
        }

        components.Add(component);
        return true;
    }

    /// <summary>
    /// Removes a component from the collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="component"/> was found and removed from the collection; otherwise <see langword="false"/>.
    /// </returns>
    public bool Remove(Component component) {
        if (component == null) {
            return false;
        }

        if (!componentInstanceIds.Remove(component.InstanceID)) {
            return false;
        }

        components.Remove(component);
        return true;
    }

    /// <summary>
    /// Finds the first component of type <typeparamref name="T"/>.
    /// Returns null if not found.
    /// </summary>
    public T Find<T>() where T : Component {
        for (int i = 0; i < components.Length; i++) {
            if (components.Buffer[i] is T c) {
                return c;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void FindAll<T>(List<T> results, bool onlyActive = false) where T : Component {
        for (int i = 0; i < components.Length; i++) {
            if (components.Buffer[i] is T c && (!onlyActive || c.Active)) {
                results.Add(c);
            }
        }
    }


    /// <summary>
    /// Finds all components of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T[] FindAll<T>(bool onlyActive = false) where T : Component {
        List<T> list = ListPool<T>.Get();

        for (int i = 0; i < components.Length; i++) {
            if (components.Buffer[i] is T c && (!onlyActive || c.Active)) {
                list.Add(c);
            }
        }

        T[] arr = list.ToArray();
        ListPool<T>.Return(list);
        return arr;
    }

    /// <summary>
    /// Returns <see langword="true"/> if this component collection contains <paramref name="component"/>.
    /// </summary>
    public bool Contains(Component component) {
        return component != null && componentInstanceIds.Contains(component.InstanceID);
    }
}
