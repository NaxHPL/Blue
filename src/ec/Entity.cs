using System.Collections.Generic;

namespace BlueFw;

public class Entity : BlueObject, IDestroyable {

    /// <summary>
    /// Gets whether this entity is active in the scene hierarchy.
    /// This is true if it's enabled and its parent is active.
    /// </summary>
    public bool Active {
        get {
            if (activeInHierachyDirty) {
                activeInHierachy = enabled && (!HasParent || Parent.Active);
                activeInHierachyDirty = false;
            }
            return activeInHierachy;
        }
    }

    /// <summary>
    /// The scene this entity is attached to.
    /// </summary>
    public Scene Scene { get; internal set; }

    /// <summary>
    /// Returns <see langword="true"/> if this entity is attached to a scene.
    /// </summary>
    public bool AttachedToScene => Scene != null;

    /// <summary>
    /// Gets or sets whether this entity is enabled.
    /// </summary>
    public bool Enabled {
        get => enabled;
        set => SetEnabled(value);
    }

    /// <summary>
    /// The name of this entity.
    /// </summary>
    public string Name;

    /// <summary>
    /// Defines whether this entity has been destroyed.
    /// </summary>
    public bool IsDestroyed { get; private set; }

    /// <summary>
    /// The transform attached to this entity.
    /// </summary>
    public readonly Transform Transform;

    /// <summary>
    /// The components attached to this entity.
    /// </summary>
    internal readonly ComponentCollection Components;

    bool enabled = true;
    bool activeInHierachy;
    bool activeInHierachyDirty = true;

    /// <summary>
    /// Creates a new <see cref="Entity"/> with the specified name.
    /// </summary>
    public Entity(string name) {
        Transform = new Transform(this);
        Components = new ComponentCollection();
        Name = string.IsNullOrEmpty(name) ? $"Entity_{InstanceID}" : name;
    }

    /// <summary>
    /// Creates a new <see cref="Entity"/>.
    /// </summary>
    public Entity() : this(null) { }

    /// <summary>
    /// Sets this entity's parent.
    /// </summary>
    public void SetParent(Entity entity) {
        if (Parent == entity) {
            return;
        }

        if (entity == null) {
            Transform.SetParent(null);
            FlagActiveInHierarchyDirty();

            if (enabled) {
                TryInvokeOnEnableOnAllChildComponents();
            }

            return;
        }

        bool activeInHierarchyBefore = Active;
        Transform.SetParent(entity.Transform);
        FlagActiveInHierarchyDirty();

        if (!activeInHierarchyBefore && Active) {
            TryInvokeOnEnableOnAllChildComponents();
        }
        else if (activeInHierarchyBefore && !Active) {
            TryInvokeOnDisableOnAllChildComponents();
        }
    }

    /// <summary>
    /// Detaches this entity from its parent.
    /// </summary>
    public void DetachFromParent() {
        SetParent(null);
    }

    /// <summary>
    /// Sets whether this entity is enabled or disabled.
    /// </summary>
    public void SetEnabled(bool enabled) {
        if (this.enabled == enabled) {
            return;
        }

        bool activeInHierarchyBefore = Active;
        this.enabled = enabled;
        FlagActiveInHierarchyDirty();

        if (!activeInHierarchyBefore && Active) {
            TryInvokeOnEnableOnAllChildComponents();
        }
        else if (activeInHierarchyBefore && !Active) {
            TryInvokeOnDisableOnAllChildComponents();
        }
    }

    void TryInvokeOnEnableOnAllChildComponents() {
        for (int i = 0; i < Components.Count; i++) {
            if (Components[i].Enabled) {
                Components[i].TryInvokeOnEnable();
            }
        }

        for (int i = ChildCount - 1; i >= 0; i--) {
            Entity child = GetChildAt(i);
            if (child.enabled) {
                child.TryInvokeOnEnableOnAllChildComponents();
            }
        }
    }

    void TryInvokeOnDisableOnAllChildComponents() {
        for (int i = 0; i < Components.Count; i++) {
            if (Components[i].Enabled) {
                Components[i].TryInvokeOnDisable();
            }
        }

        for (int i = ChildCount - 1; i >= 0; i--) {
            Entity child = GetChildAt(i);
            if (child.enabled) {
                child.TryInvokeOnDisableOnAllChildComponents();
            }
        }
    }

    void FlagActiveInHierarchyDirty() {
        activeInHierachyDirty = true;

        for (int i = 0; i < Components.Count; i++) {
            Components[i].FlagActiveInHierarchyDirty();
        }

        for (int i = ChildCount - 1; i >= 0; i--) {
            GetChildAt(i).FlagActiveInHierarchyDirty();
        }
    }

    #region Transform Passthroughs

    /// <summary>
    /// The parent of this entity.
    /// </summary>
    public Entity Parent {
        get => Transform.Parent?.Entity;
        set => SetParent(value);
    }

    /// <summary>
    /// True if this entity has a parent.
    /// </summary>
    public bool HasParent => Transform.HasParent;

    /// <summary>
    /// The number of children entities this one has.
    /// </summary>
    public int ChildCount => Transform.ChildCount;

    /// <summary>
    /// The topmost <see cref="Entity"/> in the hierarchy.
    /// </summary>
    public Entity Root => Transform.Root.Entity;

    /// <summary>
    /// Returns a boolean value that indicates whether this <see cref="Entity"/> is a child of <paramref name="entity"/>.
    /// </summary>
    /// <param name="deepSearch">If <see langword="true"/> (default), this method will also check if this <see cref="Entity"/> is a child of a child of <paramref name="entity"/>.</param>
    public bool IsChildOf(Entity entity, bool deepSearch = true) {
        return Transform.IsChildOf(entity.Transform, deepSearch);
    }

    /// <summary>
    /// Returns the child <see cref="Entity"/> at the specified index. Returns <see langword="null"/> if the index is invalid.
    /// </summary>
    public Entity GetChildAt(int index) {
        return Transform.GetChildAt(index)?.Entity;
    }

    #endregion

    #region Components

    /// <summary>
    /// Adds the specified component to this entity.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="component"/> wasn't already attached to this entity and added successfully; otherwise <see langword="false"/>.
    /// </returns>
    public bool AddComponent(Component component) {
        if (!Components.Add(component)) {
            return false;
        }

        component.DetachFromOwner();
        component.Entity = this;
        component.FlagActiveInHierarchyDirty();
        component.OnAddedToEntity(this);

        if (component.Active) {
            component.TryInvokeOnEnable();
        }
        else {
            component.TryInvokeOnDisable();
        }

        if (AttachedToScene) {
            Scene.RegisterComponent(component);
        }

        return true;
    }

    /// <summary>
    /// Adds a component class of type <typeparamref name="T"/> to this entity.
    /// </summary>
    /// <returns>The component that was added.</returns>
    public T AddComponent<T>() where T : Component, new() {
        T component = new T();
        AddComponent(component);
        return component;
    }

    /// <summary>
    /// Removes the specified component from this entity.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="component"/> was found and removed from this entity; otherwise <see langword="false"/>.
    /// </returns>
    public bool RemoveComponent(Component component) {
        if (!Components.Remove(component)) {
            return false;
        }

        component.Entity = null;
        component.FlagActiveInHierarchyDirty();
        component.OnRemovedFromEntity(this);
        component.TryInvokeOnDisable();

        if (AttachedToScene) {
            Scene.UnregisterComponent(component);
        }

        return true;
    }

    /// <summary>
    /// Removes the first occurrence of component type <typeparamref name="T"/> from this entity.
    /// </summary>
    public void RemoveComponent<T>() where T : Component {
        if (TryGetComponent(out T component)) {
            RemoveComponent(component);
        }
    }

    /// <summary>
    /// Removes all components attached to this entity.
    /// </summary>
    public void RemoveAllComponents() {
        for (int i = Components.Count - 1; i >= 0; i--) {
            RemoveComponent(Components[i]);
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> if this entity has a component of type <typeparamref name="T"/>.
    /// </summary>
    public bool HasComponent<T>() where T : Component {
        return Components.Find<T>() != null;
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> attached to this entity. If a <typeparamref name="T"/> isn't attached, returns <see langword="null"/>.
    /// </summary>
    public T GetComponent<T>() where T : Component {
        return Components.Find<T>();
    }

    /// <summary>
    /// Tries to get the first component of type <typeparamref name="T"/> attached to this entity. If successful, the component is stored in <paramref name="component"/>.
    /// </summary>
    public bool TryGetComponent<T>(out T component) where T : Component {
        component = Components.Find<T>();
        return component != null;
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void GetComponents<T>(List<T> results, bool onlyActive = false) where T : Component {
        Components.FindAll(results, onlyActive);
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T[] GetComponents<T>(bool onlyActive = false) where T : Component {
        return Components.FindAll<T>(onlyActive);
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> found on this entity or any of its children. Uses depth first search.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T GetComponentInChildren<T>(bool onlyActive = false) where T : Component {
        if (onlyActive && !Active) {
            return null;
        }

        if (TryGetComponent(out T component) && (!onlyActive || component.Active)) {
            return component;
        }

        for (int i = 0; i < ChildCount; i++) {
            T c = GetChildAt(i).GetComponentInChildren<T>(onlyActive);
            if (c != null) {
                return c;
            }
        }

        return null;
    }

    /// <summary>
    /// Adds all components of type <typeparamref name="T"/> attached to this entity or any of its children to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void GetComponentsInChildren<T>(List<T> results, bool onlyActive = false) where T : Component {
        if (onlyActive && !Active) {
            return;
        }

        Components.FindAll(results, onlyActive);

        for (int i = 0; i < ChildCount; i++) {
            GetChildAt(i).GetComponentsInChildren(results, onlyActive);
        }
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> found on this entity or any of its parents. Uses depth first search.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T GetComponentInParents<T>(bool onlyActive = false) where T : Component {
        if (TryGetComponent(out T component) && (!onlyActive || component.Active)) {
            return component;
        }

        if (HasParent) {
            return Parent.GetComponentInParents<T>(onlyActive);
        }

        return null;
    }

    /// <summary>
    /// Adds all components of type <typeparamref name="T"/> attached to this entity or any of its parents to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void GetComponentsInParents<T>(List<T> results, bool onlyActive = false) where T : Component {
        Components.FindAll(results, onlyActive);

        if (HasParent) {
            Parent.GetComponentsInParents(results, onlyActive);
        }
    }

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Called when this entity gets destroyed.
    /// </summary>
    public virtual void OnDestroy() { }

    #endregion

    /// <summary>
    /// Destroys this entity, all of its components, and all of of its children entities.
    /// </summary>
    public void Destroy() {
        if (IsDestroyed) {
            return;
        }

        IsDestroyed = true;

        for (int i = Components.Count - 1; i >= 0; i--) {
            Components[i].Destroy();
        }

        Scene.RemoveEntity(this);
        Transform.DetachFromParent();

        OnDestroy();

        for (int i = ChildCount - 1; i >= 0; i++) {
            GetChildAt(i).Destroy();
        }
    }
}
