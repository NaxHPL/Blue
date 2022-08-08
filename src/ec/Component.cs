namespace BlueFw;

/// <summary>
/// Base component class.
/// </summary>
public abstract class Component : BlueObject, IDestroyable {

    enum LifecycleEnabledMethod {
        OnEnable,
        OnDisable
    }

    /// <summary>
    /// The entity this component is attached to.
    /// </summary>
    public Entity Entity { get; internal set; }

    /// <summary>
    /// Returns <see langword="true"/> if this component is attached to an entity.
    /// </summary>
    public bool AttachedToEntity => Entity != null;

    /// <summary>
    /// Gets whether this component is active in the scene hierarchy.
    /// This is true if it's enabled and the Entity it's attached to is active.
    /// </summary>
    public bool Active {
        get {
            if (activeInHierachyDirty) {
                activeInHierachy = enabled && AttachedToEntity && Entity.Active;
                activeInHierachyDirty = false;
            }
            return activeInHierachy;
        }
    }

    /// <summary>
    /// The <see cref="Transform"/> attached to this component's entity.
    /// </summary>
    public Transform Transform => Entity?.Transform;

    /// <summary>
    /// Defines whether this component has been destroyed.
    /// </summary>
    public bool IsDestroyed { get; private set; }

    /// <summary>
    /// Defines whether this component has been initialized (had its Initialize method called).
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets or sets whether this component is enabled. A disabled component won't get updated or rendered.
    /// </summary>
    /// <remarks>
    /// A <see cref="Component"/> is always disabled if not attached to an entity.
    /// </remarks>
    public bool Enabled {
        get => enabled && AttachedToEntity;
        set => SetEnabled(value);
    }

    bool enabled = true;
    bool activeInHierachy;
    bool activeInHierachyDirty = true;
    LifecycleEnabledMethod nextEnabledMethodToInvoke = LifecycleEnabledMethod.OnEnable;

    /// <summary>
    /// Sets whether this component is locally enabled.
    /// </summary>
    public void SetEnabled(bool enabled) {
        if (this.enabled == enabled) {
            return;
        }

        this.enabled = enabled;
        FlagActiveInHierarchyDirty();

        if (enabled) {
            TryInvokeOnEnable();
        }
        else {
            TryInvokeOnDisable();
        }
    }

    internal void FlagActiveInHierarchyDirty() {
        activeInHierachyDirty = true;
    }

    /// <summary>
    /// Invoke this component's OnEnable method if allowed.
    /// </summary>
    /// <returns><see langword="true"/> if OnEnable was successfully invoked; <see langword="false"/> otherwise.</returns>
    internal bool TryInvokeOnEnable() {
        TryInvokeInitialize(); // Initialize should be called the first time this component gets enabled.

        if (nextEnabledMethodToInvoke == LifecycleEnabledMethod.OnEnable) {
            nextEnabledMethodToInvoke = LifecycleEnabledMethod.OnDisable;
            OnEnable();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Invoke this component's Initialize method if allowed.
    /// </summary>
    internal bool TryInvokeInitialize() {
        if (IsInitialized) {
            return false;
        }

        Initialize();
        IsInitialized = true;
        return true;
    }

    /// <summary>
    /// Invoke this component's OnDisable method if allowed.
    /// </summary>
    /// <returns><see langword="true"/> if OnDisable was successfully invoked; <see langword="false"/> otherwise.</returns>
    internal bool TryInvokeOnDisable() {
        if (nextEnabledMethodToInvoke == LifecycleEnabledMethod.OnDisable) {
            nextEnabledMethodToInvoke = LifecycleEnabledMethod.OnEnable;
            OnDisable();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes this component from the entity it's attached to.
    /// </summary>
    public void DetachFromEntity() {
        if (AttachedToEntity) {
            Entity.RemoveComponent(this);
        }
    }

    #region Lifecycle Methods

    /// <summary>
    /// Called just before this component is enabled for the first time. 
    /// </summary>
    /// <remarks>This is called exactly once in the component's lifetime.</remarks>
    protected virtual void Initialize() { }

    /// <summary>
    /// Called when this component or the entity this component is attached to becomes enabled.
    /// </summary>
    /// <remarks>
    /// Also called when this component gets added to an entity, after OnAddedToEntity is called.
    /// </remarks>
    protected virtual void OnEnable() { }

    /// <summary>
    /// Called when this component or the entity this component is attached to becomes disabled.
    /// </summary>
    /// <remarks>
    /// Also called when this component gets destroyed, just before OnDestroy, and when this component gets removed from an entity, just before OnRemovedFromEntity.
    /// </remarks>
    protected virtual void OnDisable() { }

    /// <summary>
    /// Called immediately after this component gets added to an entity.
    /// </summary>
    /// <param name="addedToEntity">The entity this component was added to.</param>
    public virtual void OnAddedToEntity(Entity addedToEntity) { }

    /// <summary>
    /// Called immediately after this component gets removed from an entity.
    /// </summary>
    /// <param name="removedFromEntity">The Entity this component was removed from.</param>
    public virtual void OnRemovedFromEntity(Entity removedFromEntity) { }

    /// <summary>
    /// Called when the transform of the entity this component is attached to changes.
    /// </summary>
    public virtual void OnEntityTransformChanged() { }

    /// <summary>
    /// Called when this component gets destroyed. <see cref="OnDisable"/> gets called fiirst (if enabled), then <see cref="OnDestroy"/>.
    /// </summary>
    protected virtual void OnDestroy() { }

    #endregion

    /// <summary>
    /// Removes this component from its entity and destroys it.
    /// </summary>
    public void Destroy() {
        if (IsDestroyed) {
            return;
        }

        IsDestroyed = true;

        TryInvokeOnDisable();
        OnDestroy();
        Entity.RemoveComponent(this);
    }
}
