using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BlueFw;

public class Scene {

    /// <summary>
    /// The <see cref="ContentManager"/> of this scene. Use this for scene specific content.
    /// </summary>
    /// <remarks>The content will be unloaded when the scene unloads.</remarks>
    public ContentManager Content { get; private set; }

    /// <summary>
    /// This scene's main camera.
    /// </summary>
    public readonly Camera Camera;

    /// <summary>
    /// This scene's collection of entites.
    /// </summary>
    internal readonly EntityCollection Entities;

    readonly SceneUpdater sceneUpdater = new SceneUpdater();
    readonly SceneRenderer sceneRenderer = new SceneRenderer();

    /// <summary>
    /// Creates a new <see cref="Scene"/>.
    /// </summary>
    public Scene() {
        Entities = new EntityCollection(this);

        Entity cameraEntity = new Entity("Main Camera");
        Camera = cameraEntity.AddComponent<Camera>();
        AddEntity(cameraEntity);
    }

    #region Entities/Components

    /// <summary>
    /// Adds an <see cref="Entity"/> and all of its children entities to this scene.
    /// </summary>
    public void AddEntity(Entity entity) {
        if (!Entities.Add(entity)) {
            return;
        }

        entity.Scene = this;
        RegisterComponents(entity.Components);

        for (int i = 0; i < entity.ChildCount; i++) {
            AddEntity(entity.GetChildAt(i));
        }
    }

    /// <summary>
    /// Removes an <see cref="Entity"/> and all of its children entities from the scene.
    /// </summary>
    /// <returns><see langword="true"/> if <paramref name="entity"/> was found and removed; otherwise <see langword="false"/>.</returns>
    public bool RemoveEntity(Entity entity) {
        if (!Entities.Remove(entity)) {
            return false;
        }

        entity.Scene = null;
        UnregisterComponents(entity.Components);

        for (int i = 0; i < entity.ChildCount; i++) {
            RemoveEntity(entity.GetChildAt(i));
        }

        return true;
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

    /// <summary>
    /// Finds a component in this scene of type <typeparamref name="T"/>.
    /// </summary>
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
    /// Apply any changes made to update orders. The new order will be reflected in the next update cycle.
    /// </summary>
    public void ApplyUpdateOrderChanges() {
        sceneUpdater.FlagUpdateOrderDirty();
    }

    internal void Update() {
        sceneUpdater.Update();
    }

    /// <summary>
    /// Apply any changes made to render orders. The new order will be reflected in the next update cycle.
    /// </summary>
    public void ApplyRenderOrderChanges() {
        sceneRenderer.FlagRenderOrderDirty();
    }

    internal void Render(SpriteBatch spriteBatch) {
        sceneRenderer.Render(spriteBatch);
    }

    #endregion

    #region Lifecycle Methods

    internal void Load() {
        Content = new ContentManager(Blue.Instance.Services, Blue.Instance.Content.RootDirectory);
        OnLoad();
    }

    /// <summary>
    /// Called when this scene is loaded and becomes the active scene.
    /// </summary>
    protected virtual void OnLoad() { }

    public void Unload() {
        for (int i = 0; i < Entities.Count; i++) {
            Entities[i].Destroy();
        }

        Content?.Dispose();

        OnUnload();
    }

    /// <summary>
    /// Called when this scene is unloaded and is no longer the active scene.
    /// </summary>
    protected virtual void OnUnload() { }

    #endregion
}
