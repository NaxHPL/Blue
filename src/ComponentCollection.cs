using System;
using System.Collections.Generic;

namespace BlueFw;

internal class ComponentCollection {

    /// <summary>
    /// The number of components in this <see cref="ComponentCollection"/>.
    /// </summary>
    public int Count => allComponents.Length;

    /// <summary>
    /// Gets the <see cref="Component"/> at the specified index.
    /// </summary>
    public Component this[int index] {
        get {
            if (index >= allComponents.Length) {
                throw new IndexOutOfRangeException();
            }

            return allComponents.Buffer[index];
        }
    }

    /// <summary>
    /// The entity this <see cref="ComponentCollection"/> is attached to.
    /// </summary>
    public readonly Entity Entity;

    readonly FastList<Component> allComponents = new FastList<Component>();
    readonly HashSet<uint> allComponentsInstanceIds = new HashSet<uint>(); // More efficient to search this instead of the allComponents list, but gotta keep them in sync
    readonly HashSet<Component> componentsToAdd = new HashSet<Component>();
    readonly HashSet<Component> componentsToRemove = new HashSet<Component>();

    static readonly List<Component> tempList = new List<Component>(); // A reusable list used by various methods

    /// <summary>
    /// Creates a new <see cref="ComponentCollection"/>.
    /// </summary>
    /// <param name="entity">The entity this <see cref="ComponentCollection"/> is attached to.</param>
    public ComponentCollection(Entity entity) {
        Entity = entity;
    }

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

        if (componentsToRemove.Remove(component)) {
            return true;
        }

        if (allComponentsInstanceIds.Contains(component.InstanceID)) {
            return false;
        }

        return componentsToAdd.Add(component);
    }

    /// <summary>
    /// Removes a component from the collection.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="component"/> was found and removed from the collection; otherwise <see langword="false"/>.
    /// </returns>
    public bool Remove(Component component) {
        if (componentsToAdd.Remove(component)) {
            return true;
        }

        if (!allComponentsInstanceIds.Contains(component.InstanceID)) {
            return false;
        }

        return componentsToRemove.Add(component);
    }

    /// <summary>
    /// Gets the first component of type <typeparamref name="T"/>.
    /// </summary>
    public T Get<T>() where T : Component {
        for (int i = 0; i < allComponents.Length; i++) {
            Component component = allComponents.Buffer[i];
            if (component is T c && !componentsToRemove.Contains(component)) {
                return c;
            }
        }

        foreach (Component component in componentsToAdd) {
            if (component is T c) {
                return c;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all components of type <typeparamref name="T"/> to <paramref name="results"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for components which are active in the hierarchy.
    /// Set <paramref name="includeInactive"/> to <see langword="true"/> to include inactive components in the search.
    /// </remarks>
    /// <param name="includeInactive">(Optional) Include inactive components in the search.</param>
    public void GetAll<T>(List<T> results, bool includeInactive = false) where T : Component {
        for (int i = 0; i < allComponents.Length; i++) {
            Component component = allComponents.Buffer[i];
            if (component is T c && (includeInactive || component.ActiveInHierarchy) && !componentsToRemove.Contains(component)) {
                results.Add(c);
            }
        }

        foreach (Component component in componentsToAdd) {
            if (component is T c && (includeInactive || component.ActiveInHierarchy)) {
                results.Add(c);
            }
        }
    }


    /// <summary>
    /// Returns all components of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for components which are active in the hierarchy.
    /// Set <paramref name="includeInactive"/> to <see langword="true"/> to include inactive components in the search.
    /// </remarks>
    /// <param name="includeInactive">(Optional) Include inactive components in the search.</param>
    public T[] GetAll<T>(bool includeInactive = false) where T : Component {
        for (int i = 0; i < allComponents.Length; i++) {
            Component component = allComponents.Buffer[i];
            if (component is T && (includeInactive || component.ActiveInHierarchy) && !componentsToRemove.Contains(component)) {
                tempList.Add(component);
            }
        }

        foreach (Component component in componentsToAdd) {
            if (component is T && (includeInactive || component.ActiveInHierarchy)) {
                tempList.Add(component);
            }
        }

        T[] arr = (T[])tempList.ToArray();
        tempList.Clear();
        return arr;
    }

    /// <summary>
    /// Returns <see langword="true"/> if this component collection contains <paramref name="component"/>.
    /// </summary>
    public bool Contains(Component component) {
        return
            !componentsToRemove.Contains(component) &&
            (allComponentsInstanceIds.Contains(component.InstanceID) || componentsToAdd.Contains(component));
    }
    internal void OnEntityTransformChanged() {
        for (int i = 0; i < allComponents.Length; i++) {
            allComponents[i].OnEntityTransformChanged();
        }

        foreach (Component component in componentsToAdd) {
            component.OnEntityTransformChanged();
        }
    }

    // TODO: CALL THIS AT END OF UPDATE
    internal void ApplyPendingChanges() {
        if (componentsToRemove.Count > 0) {
            foreach (Component component in componentsToRemove) {
                RemoveImmediate(component);
            }
            componentsToRemove.Clear();
        }

        if (componentsToAdd.Count > 0) {
            foreach (Component component in componentsToAdd) {
                AddImmediate(component);
            }
            componentsToAdd.Clear();
        }
    }

    void RemoveImmediate(Component component) {
        allComponents.Remove(component);
        allComponentsInstanceIds.Remove(component.InstanceID);

        component.TryInvokeOnDisable();
    }

    void AddImmediate(Component component) {
        allComponents.Add(component);
        allComponentsInstanceIds.Add(component.InstanceID);

        if (component.ActiveInHierarchy) {
            component.TryInvokeOnEnable();
        }
        else {
            component.TryInvokeOnDisable();
        }
    }
}
