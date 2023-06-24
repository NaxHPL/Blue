using BlueFw.Coroutines;
using BlueFw.Utils;
using System;
using System.Collections.Generic;

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
        StopAllCoroutines();
        return true;
    }

    internal void InvokeOnEntityTransformChanged(Transform.ComponentFlags changedFlags) {
        OnEntityTransformChanged(changedFlags);
    }

    #region Entity Passthroughs

    /// <summary>
    /// Returns <see langword="true"/> if this component is attached to an entity and the entity has a component of type <typeparamref name="T"/>.
    /// </summary>
    public bool HasComponent<T>() where T : Component {
        return AttachedToEntity && Entity.HasComponent<T>();
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> attached to this component's entity.
    /// If this component isn't attached to an entity or the entity has no <typeparamref name="T"/> component, returns <see langword="null"/>.
    /// </summary>
    public T GetComponent<T>() where T : Component {
        return Entity?.GetComponent<T>();
    }

    /// <summary>
    /// Tries to get the first component of type <typeparamref name="T"/> attached to this component's entity. If successful, the component is stored in <paramref name="component"/>.
    /// </summary>
    public bool TryGetComponent<T>(out T component) where T : Component {
        if (AttachedToEntity) {
            return Entity.TryGetComponent(out component);
        }

        component = null;
        return false;
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> attached to this component's entity and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void GetComponents<T>(List<T> results, bool onlyActive = false) where T : Component {
        if (AttachedToEntity) {
            Entity.GetComponents(results, onlyActive);
        }
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> attached to this component's entity.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T[] GetComponents<T>(bool onlyActive = false) where T : Component {
        return AttachedToEntity ? Entity.GetComponents<T>(onlyActive) : Array.Empty<T>();
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> found on this component's entity or any of its children. Uses depth first search.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T GetComponentInChildren<T>(bool onlyActive = false) where T : Component {
        return AttachedToEntity ? Entity.GetComponentInChildren<T>(onlyActive) : null;
    }

    /// <summary>
    /// Adds all components of type <typeparamref name="T"/> attached to this component's entity or any of its children to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void GetComponentsInChildren<T>(List<T> results, bool onlyActive = false) where T : Component {
        if (AttachedToEntity) {
            Entity.GetComponentsInChildren(results, onlyActive);
        }
    }

    /// <summary>
    /// Returns the first component of type <typeparamref name="T"/> found on this component's entity or any of its parents. Uses depth first search.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T GetComponentInParents<T>(bool onlyActive = false) where T : Component {
        return AttachedToEntity ? Entity.GetComponentInParents<T>(onlyActive) : null;
    }

    /// <summary>
    /// Adds all components of type <typeparamref name="T"/> attached to this component's entity or any of its parents to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void GetComponentsInParents<T>(List<T> results, bool onlyActive = false) where T : Component {
        if (AttachedToEntity) {
            Entity.GetComponentsInParents(results, onlyActive);
        }
    }

    #endregion

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
    /// <param name="changedComponentFlags">The transform components that changed.</param>
    protected virtual void OnEntityTransformChanged(Transform.ComponentFlags changedComponentFlags) { }

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

    #region Coroutines

    FastList<Coroutine> nonTaggedCoroutines;
    Dictionary<string, FastList<Coroutine>> coroutinesByTag;

    /// <summary>
    /// Starts a coroutine.
    /// </summary>
    /// <remarks>
    /// Untagged coroutines cannot be paused/resumed and can only be ended early using <see cref="StopAllCoroutines"/>.
    /// </remarks>
    protected void StartCoroutine(IEnumerator<IYieldInstruction> coroutine) {
        StartCoroutine(coroutine, null);
    }

    /// <summary>
    /// Starts a coroutine with the given tag.
    /// </summary>
    protected void StartCoroutine(IEnumerator<IYieldInstruction> coroutine, string tag) {
        ArgumentNullException.ThrowIfNull(coroutine, nameof(coroutine));

        Coroutine cr = Pool<Coroutine>.Get();
        cr.Initialize(tag, coroutine, this);
        cr.Advance(); // advance to the first yield instruction
        Blue.Instance.CoroutineManager.Register(cr);

        if (tag == null) {
            nonTaggedCoroutines ??= new FastList<Coroutine>();
            nonTaggedCoroutines.Add(cr);
        }
        else {
            coroutinesByTag ??= new Dictionary<string, FastList<Coroutine>>();
            if (!coroutinesByTag.ContainsKey(tag)) {
                coroutinesByTag.Add(tag, FastListPool<Coroutine>.Get());
            }

            coroutinesByTag[tag].Add(cr);
        }
    }

    /// <summary>
    /// Pauses all coroutines running on this component with the given tag.
    /// </summary>
    protected void PauseCoroutines(string tag) {
        SetCoroutinesPaused(tag, true);
    }

    /// <summary>
    /// Resumes all coroutines running on this component with the given tag.
    /// </summary>
    protected void ResumeCoroutines(string tag) {
        SetCoroutinesPaused(tag, false);
    }

    void SetCoroutinesPaused(string tag, bool paused) {
        if (coroutinesByTag == null) {
            return;
        }

        if (!coroutinesByTag.TryGetValue(tag, out FastList<Coroutine> coroutines)) {
            return;
        }

        for (int i = 0; i < coroutines.Length; i++) {
            coroutines.Buffer[i].IsPaused = paused;
        }
    }

    /// <summary>
    /// Stops all coroutines running on this component with the given tag.
    /// </summary>
    protected void StopCoroutines(string tag) {
        if (coroutinesByTag == null) {
            return;
        }

        if (!coroutinesByTag.Remove(tag, out FastList<Coroutine> coroutines)) {
            return;
        }

        for (int i = 0; i < coroutines.Length; i++) {
            Blue.Instance.CoroutineManager.Unregister(coroutines.Buffer[i]);
        }

        FastListPool<Coroutine>.Return(coroutines);
    }

    /// <summary>
    /// Stops all coroutines running on this component.
    /// </summary>
    protected void StopAllCoroutines() {
        if (nonTaggedCoroutines != null) {
            for (int i = 0; i < nonTaggedCoroutines.Length; i++) {
                Blue.Instance.CoroutineManager.Unregister(nonTaggedCoroutines.Buffer[i]);
            }

            nonTaggedCoroutines.Clear();
        }

        if (coroutinesByTag != null) {
            foreach (FastList<Coroutine> coroutines in coroutinesByTag.Values) {
                for (int i = 0; i < coroutines.Length; i++) {
                    Blue.Instance.CoroutineManager.Unregister(coroutines.Buffer[i]);
                }

                FastListPool<Coroutine>.Return(coroutines);
            }

            coroutinesByTag.Clear();
        }
    }

    internal void CoroutineDone(Coroutine cr) {
        if (cr.Tag == null) {
            nonTaggedCoroutines?.Remove(cr);
        }
        else if (coroutinesByTag != null) {
            if (coroutinesByTag.TryGetValue(cr.Tag, out FastList<Coroutine> coroutines)) {
                coroutines.Remove(cr);

                if (coroutines.Length == 0) {
                    coroutinesByTag.Remove(cr.Tag);
                    FastListPool<Coroutine>.Return(coroutines);
                }
            }
        }
    }

    #endregion
}
