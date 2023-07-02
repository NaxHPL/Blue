using BlueFw.Content;
using System;
using System.Collections.Generic;

namespace BlueFw;

public class Scene {

    /// <summary>
    /// Gets whether this scene is the active scene.
    /// </summary>
    public bool IsActiveScene => this == Blue.Instance.ActiveScene;

    /// <summary>
    /// Has the scene finished loading?
    /// </summary>
    internal bool IsLoaded { get; private set; }

    /// <summary>
    /// The content manager of this scene. Use this for scene specific content.
    /// </summary>
    /// <remarks>The content will be unloaded when the scene unloads.</remarks>
    public BlueContent Content { get; private set; }

    /// <summary>
    /// This scene's main camera.
    /// </summary>
    public readonly Camera Camera;

    /// <summary>
    /// This scene's collection of entites.
    /// </summary>
    internal readonly EntityCollection Entities = new EntityCollection();

    /// <summary>
    /// This scene's global scene components.
    /// </summary>
    internal readonly ComponentCollection SceneComponents = new ComponentCollection();

    readonly SceneUpdater sceneUpdater = new SceneUpdater();
    readonly SceneRenderer sceneRenderer = new SceneRenderer();

    /// <summary>
    /// Creates a new <see cref="Scene"/>.
    /// </summary>
    public Scene() {
        Camera = CreateEntity("Main Camera").AddComponent<Camera>();
    }

    #region Entities

    /// <summary>
    /// Creates an <see cref="Entity"/> and adds it to this scene.
    /// </summary>
    public Entity CreateEntity(string name = null) {
        return CreateEntity<Entity>(name);
    }

    /// <summary>
    /// Creates an entity of type <typeparamref name="T"/> and adds it to this scene.
    /// </summary>
    public T CreateEntity<T>(string name = null) where T : Entity, new() {
        T entity = new T();

        if (!string.IsNullOrEmpty(name)) {
            entity.Name = name;
        }

        AddEntity(entity);
        return entity;
    }

    /// <summary>
    /// Adds an entity to the scene.
    /// </summary>
    internal void AddEntity(Entity entity) {
        if (!Entities.Add(entity)) {
            return;
        }

        if (entity.AttachedToScene) {
            entity.Scene.RemoveEntity(entity);
        }

        entity.Scene = this;
        RegisterComponents(entity.Components);

        if (IsLoaded) {
            // If we're adding this entity while the scene is loaded, call awake/onactive on components
            entity.TryInvokeAwakeOnComponents();
            entity.UpdateActive();
        }

        entity.OnAddedToScene();

        for (int i = 0; i < entity.ChildCount; i++) {
            AddEntity(entity.GetChildAt(i));
        }
    }

    /// <summary>
    /// Removes an <see cref="Entity"/> and all of its children entities from the scene.
    /// </summary>
    internal void RemoveEntity(Entity entity) {
        if (!Entities.Remove(entity)) {
            return;
        }

        entity.Scene = null;
        UnregisterComponents(entity.Components);

        if (IsLoaded) {
            entity.UpdateActive();
        }

        for (int i = 0; i < entity.ChildCount; i++) {
            RemoveEntity(entity.GetChildAt(i));
        }
    }

    /// <summary>
    /// Finds an entity in this scene with the specified name.
    /// </summary>
    public Entity FindEntity(string name) {
        return Entities.Find(name);
    }

    /// <summary>
    /// Finds an entity in this scene with the specified instance ID.
    /// </summary>
    public Entity FindEntity(uint instanceId) {
        return Entities.Find(instanceId);
    }

    /// <summary>
    /// Finds an entity in this scene of type <typeparamref name="T"/>.
    /// </summary>
    public T FindEntity<T>() where T : Entity {
        return Entities.Find<T>();
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/> and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider entities which are active in the hierarchy.</param>
    public void FindEntities<T>(List<T> results, bool onlyActive = false) where T : Entity {
        Entities.FindAll(results, onlyActive);
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider entities which are active in the hierarchy.</param>
    public T[] FindEntities<T>(bool onlyActive = false) where T : Entity {
        return Entities.FindAll<T>(onlyActive);
    }

    #endregion

    #region Components

    /// <summary>
    /// Adds a scene component of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// Scene components are ideal for components that will last the entire lifetime of the scene and don't require a <see cref="Transform"/>. <br/>
    /// They must not be renderable (attach renderables to entities).
    /// </remarks>
    public T AddSceneComponent<T>() where T : Component, new() {
        T component = new T();
        AddSceneComponentInternal(component);
        return component;
    }

    void AddSceneComponentInternal(Component component) {
        if (component is IRenderable) {
            throw new ArgumentException("Scene components must not be an IRenderable! Attach renderables to entities.", nameof(component));
        }

        if (!SceneComponents.Add(component)) {
            return;
        }

        component.Scene = this;

        if (IsActiveScene && IsLoaded) {
            component.TryInvokeAwake();
        }

        component.TryInvokeOnActive();

        RegisterComponent(component);
    }

    internal void RemoveSceneComponent(Component component) {
        if (!SceneComponents.Remove(component)) {
            return;
        }

        component.Scene = null;
        component.TryInvokeOnInactive();

        UnregisterComponent(component);
    }

    /// <summary>
    /// Gets a scene component of type <typeparamref name="T"/>.
    /// Returns null if not found.
    /// </summary>
    public T GetSceneComponent<T>() where T : Component {
        return SceneComponents.Find<T>();
    }

    /// <summary>
    /// Tries to get the first scene component of type <typeparamref name="T"/>. If successful, the component is stored in <paramref name="component"/>.
    /// </summary>
    public bool TryGetSceneComponent<T>(out T component) where T : Component {
        component = SceneComponents.Find<T>();
        return component != null;
    }

    /// <summary>
    /// Finds a component in this scene of type <typeparamref name="T"/>.
    /// Returns null if not found.
    /// </summary>
    /// <remarks>
    /// This searches for components attached to entities in the scene.
    /// To search for scene components, use <see cref="GetSceneComponent{T}"/>.
    /// </remarks>
    public T FindComponent<T>() where T : Component {
        return Entities.FindComponent<T>();
    }

    /// <summary>
    /// Finds all components in this scene of type <typeparamref name="T"/> and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void FindComponents<T>(List<T> results, bool onlyActive = false) where T : Component {
        Entities.FindComponents(results, onlyActive);
    }

    /// <summary>
    /// Finds all components in this scene of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T[] FindComponents<T>(bool onlyActive = false) where T : Component {
        return Entities.FindComponents<T>(onlyActive);
    }

    #endregion

    #region Update/Render

    internal void RegisterComponents(ComponentCollection components) {
        for (int i = 0; i < components.Count; i++) {
            RegisterComponent(components[i]);
        }
    }

    internal void UnregisterComponents(ComponentCollection components) {
        for (int i = 0; i < components.Count; i++) {
            UnregisterComponent(components[i]);
        }
    }

    internal void RegisterComponent(Component component) {
        if (component is IUpdatable updatable) {
            sceneUpdater.Register(updatable);
        }
        if (component is IRenderable renderable) {
            sceneRenderer.Register(renderable);
        }
    }

    internal void UnregisterComponent(Component component) {
        if (component is IUpdatable updatable) {
            sceneUpdater.Unregister(updatable);
        }
        if (component is IRenderable renderable) {
            sceneRenderer.Unregister(renderable);
        }
    }

    /// <summary>
    /// Flag that the update order of components have changed. The new order will be reflected in the next update cycle.
    /// </summary>
    public void UpdateOrderDirty() {
        sceneUpdater.FlagItemOrderDirty();
    }

    internal void Update() {
        sceneUpdater.Update();
    }

    /// <summary>
    /// Flag that the render order of components have changed. The new order will be reflected in the next update cycle.
    /// </summary>
    public void RenderOrderDirty() {
        sceneRenderer.FlagItemOrderDirty();
    }

    internal void Render(BlueSpriteBatch spriteBatch) {
        sceneRenderer.Render(spriteBatch, Camera);
    }

    #endregion

    internal void Load() {
        IsLoaded = false;

        Content = new BlueContent(Blue.Instance.Content);
        OnLoad();

        IsLoaded = true;

        for (int i = 0; i < SceneComponents.Count; i++) {
            SceneComponents[i].TryInvokeAwake();
            SceneComponents[i].TryInvokeOnActive();
        }

        for (int i = 0; i < Entities.Count; i++) {
            Entities[i].TryInvokeAwakeOnComponents();
            Entities[i].UpdateActive();
        }

        OnLoadFinished();
    }

    internal void Unload() {
        for (int i = SceneComponents.Count - 1; i >= 0; i--) {
            BlueObject.DestroyImmediate(SceneComponents[i]);
        }

        for (int i = Entities.Count - 1; i >= 0; i--) {
            BlueObject.DestroyImmediate(Entities[i]);
        }

        Content.Dispose();
        Content = null;

        OnUnload();

        IsLoaded = false;
    }

    #region Lifecycle Methods

    /// <summary>
    /// Called when this scene is loaded and becomes the active scene.
    /// Use this to create entities and load scene content.
    /// </summary>
    protected virtual void OnLoad() { }

    /// <summary>
    /// Called when the scene is finished loading and Awake/OnActive have been called on all components.
    /// </summary>
    protected virtual void OnLoadFinished() { }

    /// <summary>
    /// Called when this scene is unloaded and is no longer the active scene.
    /// </summary>
    protected virtual void OnUnload() { }

    #endregion
}
