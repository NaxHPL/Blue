namespace BlueFw;

/// <summary>
/// Base component class.
/// </summary>
public abstract class Component : BlueObject {

    enum LifecycleEnabledMethod {
        OnActive,
        OnInactive
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
    /// The Scene this component is attached to. It will be attached to a Scene if it's a Scene component.
    /// </summary>
    public Scene Scene { get; internal set; }

    /// <summary>
    /// Returns <see langword="true"/> if this component is a scene component.
    /// </summary>
    public bool AttachedToScene => Scene != null;

    /// <summary>
    /// Gets whether this component is active in the scene hierarchy.
    /// This is true if it's enabled and the Entity it's attached to is active
    /// OR it's enabled and is a scene component for the active scene.
    /// </summary>
    /// <remarks>
    /// An inactive component won't get updated or rendered
    /// </remarks>
    public bool Active => enabled && ((AttachedToEntity && Entity.Active) || (AttachedToScene && Scene.IsActive));

    /// <summary>
    /// The <see cref="Transform"/> attached to this component's entity.
    /// </summary>
    public Transform Transform => Entity?.Transform;

    /// <summary>
    /// Gets or sets whether this component is enabled.
    /// </summary>
    public bool Enabled {
        get => enabled;
        set => SetEnabled(value);
    }

    bool isAwake = false;
    bool hasStarted = false;
    bool enabled = true;
    LifecycleEnabledMethod nextActiveMethodToInvoke = LifecycleEnabledMethod.OnActive;

    internal bool TryInvokeAwake() {
        if (isAwake) {
            return false;
        }

        isAwake = true;
        Awake();
        return true;
    }

    /// <summary>
    /// Sets whether this component is locally enabled.
    /// </summary>
    public void SetEnabled(bool enabled) {
        if (this.enabled == enabled) {
            return;
        }

        this.enabled = enabled;

        if (Active) {
            TryInvokeOnActive();
        }
        else {
            TryInvokeOnInactive();
        }
    }

    /// <summary>
    /// Invoke this component's OnEnable method if allowed.
    /// </summary>
    /// <returns><see langword="true"/> if OnEnable was successfully invoked; <see langword="false"/> otherwise.</returns>
    internal bool TryInvokeOnActive() {
        if (nextActiveMethodToInvoke != LifecycleEnabledMethod.OnActive || !Active) {
            return false;
        }

        nextActiveMethodToInvoke = LifecycleEnabledMethod.OnInactive;
        TryInvokeStart();
        OnActive();
        return true;
    }

    internal bool TryInvokeStart() {
        if (hasStarted) {
            return false;
        }

        hasStarted = true;
        Start();
        return true;
    }

    /// <summary>
    /// Invoke this component's OnDisable method if allowed.
    /// </summary>
    /// <returns><see langword="true"/> if OnDisable was successfully invoked; <see langword="false"/> otherwise.</returns>
    internal bool TryInvokeOnInactive() {
        if (nextActiveMethodToInvoke != LifecycleEnabledMethod.OnInactive || Active) {
            return false;
        }

        nextActiveMethodToInvoke = LifecycleEnabledMethod.OnActive;
        OnInactive();
        return true;
    }

    #region Lifecycle Methods

    /// <summary>
    /// Called when the scene first loads. If the entity this component is attached to is inactive during scene load, Awake will not be called until it is made active.
    /// </summary>
    /// <remarks>
    /// Called exactly once in the lifetime of a Component.
    /// </remarks>
    protected virtual void Awake() { }

    /// <summary>
    /// Called just before this component becomes active for the first time.
    /// </summary>
    /// <remarks>
    /// Called exactly once in the lifetime of a Component.
    /// </remarks>
    protected virtual void Start() { }

    /// <summary>
    /// Called when this component becomes active.
    /// </summary>
    protected virtual void OnActive() { }

    /// <summary>
    /// Called when the transform of the entity this component is attached to changes.
    /// </summary>
    public virtual void OnEntityTransformChanged() { }

    /// <summary>
    /// Called when this component becomes inactive.
    /// </summary>
    protected virtual void OnInactive() { }

    /// <summary>
    /// Called when this component gets destroyed.
    /// </summary>
    /// <remarks>
    /// This will only be called if the component is awake (<see cref="Awake"/> was called previously).
    /// </remarks>
    protected virtual void OnDestroy() { }

    #endregion

    /// <summary>
    /// Destroys this component and removes it from its entity.
    /// </summary>
    protected override void Destroy() {
        Entity?.RemoveComponent(this);
        Scene?.RemoveSceneComponent(this);

        TryInvokeOnInactive();

        if (isAwake) {
            OnDestroy();
        }
    }
}
