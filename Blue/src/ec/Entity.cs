using System;
using System.Collections.Generic;

namespace BlueFw;

public class Entity : BlueObject {

    /// <summary>
    /// Gets whether this entity is active in the scene hierarchy.
    /// This is true if it's attached to an active scene, enabled, and its parent is active.
    /// </summary>
    public bool Active { get; private set; }

    /// <summary>
    /// The scene this entity is attached to.
    /// </summary>
    public Scene Scene {
        get => scene;
        internal set => SetScene(value);
    }

    /// <summary>
    /// Returns <see langword="true"/> if this entity is attached to a scene.
    /// </summary>
    public bool AttachedToScene => scene != null;

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
    /// The transform attached to this entity.
    /// </summary>
    public readonly Transform Transform;

    /// <summary>
    /// The components attached to this entity.
    /// </summary>
    internal readonly ComponentCollection Components;

    Scene scene;
    bool isAwake = false;
    bool enabled = true;

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

    internal bool TryInvokeAwake() {
        if (isAwake || !Active) {
            return false;
        }

        isAwake = true;

        for (int i = 0; i < Components.Count; i++) {
            Components[i].TryInvokeAwake();
        }

        return true;
    }

    internal void SetScene(Scene scene) {
        if (this.scene == scene) {
            return;
        }

        this.scene = scene;
        UpdateActive();
    }

    /// <summary>
    /// Sets this entity's parent.
    /// </summary>
    public void SetParent(Entity entity) {
        if (Parent == entity) {
            return;
        }

        if (entity == null) {
            Transform.SetParent(null);
        }
        else {
            Transform.SetParent(entity.Transform);
        }

        if (entity.AttachedToScene && entity.scene != scene) {
            entity.scene.AddEntity(this);
        }

        UpdateActive();
    }

    /// <summary>
    /// Sets whether this entity is enabled or disabled.
    /// </summary>
    public void SetEnabled(bool enabled) {
        if (this.enabled == enabled) {
            return;
        }

        this.enabled = enabled;
        UpdateActive();
    }

    internal void UpdateActive() {
        bool wasActiveBefore = Active;
        Active = enabled && AttachedToScene && scene.IsActive && (!HasParent || Parent.Active);

        // Return early if active state didn't change
        if (wasActiveBefore == Active) {
            return;
        }

        TryInvokeAwake();

        // Invoke OnActive or OnInactive for all components
        for (int i = 0; i < Components.Count; i++) {
            if (Components[i].Active) {
                Components[i].TryInvokeOnActive();
            }
            else {
                Components[i].TryInvokeOnInactive();
            }
        }

        for (int i = ChildCount - 1; i >= 0; i--) {
            GetChildAt(i).UpdateActive();
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
    /// Adds a component class of type <typeparamref name="T"/> to this entity.
    /// </summary>
    /// <returns>The component that was added.</returns>
    public T AddComponent<T>() where T : Component, new() {
        T component = new T();
        AddComponentInternal(component);
        return component;
    }

    void AddComponentInternal(Component component) {
        if (!Components.Add(component)) {
            return;
        }

        component.Entity = this;

        if (isAwake && Active) {
            component.TryInvokeAwake();
        }

        if (component.Active) {
            component.TryInvokeOnActive();
        }

        if (AttachedToScene) {
            scene.RegisterComponent(component);
        }
    }

    internal bool RemoveComponent(Component component) {
        if (!Components.Remove(component)) {
            return false;
        }

        component.Entity = null;
        component.TryInvokeOnInactive();

        if (AttachedToScene) {
            scene.UnregisterComponent(component);
        }

        return true;
    }

    /// <summary>
    /// Returns <see langword="true"/> if this entity has a component of type <typeparamref name="T"/>.
    /// </summary>
    public bool HasComponent<T>() where T : Component {
        return TryGetComponent<T>(out _);
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
    /// Called when this entity gets added to a scene.
    /// </summary>
    public virtual void OnAddedToScene() { }

    /// <summary>
    /// Destroys this entity, all of its components, and all of of its children entities.
    /// </summary>
    protected override void Destroy() {
        for (int i = ChildCount - 1; i >= 0; i++) {
            GetChildAt(i).Destroy();
        }

        for (int i = Components.Count - 1; i >= 0; i--) {
            DestroyImmediate(Components[i]);
        }

        Transform.DetachFromParent();
        scene?.RemoveEntity(this);
    }

    #endregion
}
