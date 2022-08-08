using System;
using System.Collections.Generic;

namespace BlueFw;

internal class ComponentCollection {

    /// <summary>
    /// The entity this <see cref="ComponentCollection"/> is attached to.
    /// </summary>
    public readonly Entity Entity;

    /// <summary>
    /// The number of components in this <see cref="ComponentCollection"/>.
    /// </summary>
    public int Count => components.Length;

    /// <summary>
    /// Gets the <see cref="Component"/> at the specified index.,
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

    static readonly List<Component> reusableComponentList = new List<Component>(); // A reusable list used by various methods

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
    /// Gets the first component of type <typeparamref name="T"/>.
    /// </summary>
    public T Get<T>() where T : Component {
        for (int i = 0; i < components.Length; i++) {
            if (components.Buffer[i] is T c) {
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
        for (int i = 0; i < components.Length; i++) {
            Component component = components.Buffer[i];
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
        for (int i = 0; i < components.Length; i++) {
            Component component = components.Buffer[i];
            if (component is T && (includeInactive || component.ActiveInHierarchy) && !componentsToRemove.Contains(component)) {
                reusableComponentList.Add(component);
            }
        }

        foreach (Component component in componentsToAdd) {
            if (component is T && (includeInactive || component.ActiveInHierarchy)) {
                reusableComponentList.Add(component);
            }
        }

        T[] arr = (T[])reusableComponentList.ToArray();
        reusableComponentList.Clear();
        return arr;
    }

    /// <summary>
    /// Returns <see langword="true"/> if this component collection contains <paramref name="component"/>.
    /// </summary>
    public bool Contains(Component component) {
        return
            !componentsToRemove.Contains(component) &&
            (componentInstanceIds.Contains(component.InstanceID) || componentsToAdd.Contains(component));
    }
    internal void OnEntityTransformChanged() {
        for (int i = 0; i < components.Length; i++) {
            components[i].OnEntityTransformChanged();
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
        components.Remove(component);
        componentInstanceIds.Remove(component.InstanceID);

        component.TryInvokeOnDisable();
    }

    void AddImmediate(Component component) {
        components.Add(component);
        componentInstanceIds.Add(component.InstanceID);

        if (component.ActiveInHierarchy) {
            component.TryInvokeOnEnable();
        }
        else {
            component.TryInvokeOnDisable();
        }
    }
}
