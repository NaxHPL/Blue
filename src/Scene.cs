using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Blue;

public class Scene {

    /// <summary>
    /// The Blue instance running this scene. If this scene is not the active scene, this will be null.
    /// </summary>
    public BlueInstance BlueInstance { get; private set; }

    /// <summary>
    /// The <see cref="ContentManager"/> of this scene. Use this for scene specific content.
    /// </summary>
    /// <remarks>The content will be unloaded when the scene unloads.</remarks>
    public ContentManager Content { get; private set; }

    /// <summary>
    /// This scene's collection of entites.
    /// </summary>
    public readonly EntityCollection Entities;

    /// <summary>
    /// This scene's main camera.
    /// </summary>
    public readonly Camera Camera;

    readonly RenderableCollection renderables;
    readonly FastList<RenderGroup> renderGroups = new FastList<RenderGroup>();

    RenderTarget2D sceneRenderTarget;

    bool renderGroupsNeedSorting;

    /// <summary>
    /// Creates a new <see cref="Scene"/>.
    /// </summary>
    public Scene() {
        Entities = new EntityCollection(this);
        renderables = new RenderableCollection(this);

        Entity cameraEntity = new Entity("Main Camera");
        Camera = cameraEntity.AddComponent<Camera>();
        AddEntity(cameraEntity);
    }

    /// <summary>
    /// Adds a render group to the scene.
    /// </summary>
    public void AddRenderGroup(RenderGroup renderGroup) {
        if (renderGroups.Contains(renderGroup)) {
            return;
        }

        renderGroups.Add(renderGroup);
        renderGroupsNeedSorting = true;
    }

    /// <summary>
    /// Removes a render group from the scene.
    /// </summary>
    public void RemoveRenderGroup(RenderGroup renderGroup) {
        renderGroups.Remove(renderGroup);
    }

    internal void Update() {
        Entities.Update();
    }

    internal void Render(SpriteBatch spriteBatch) {
        if (renderGroups.Length == 0) {
            Log.Error("The scene has no render groups!");
            return;
        }

        renderables.EnsureSorted();
        EnsureRenderGroupsSorted();

        //for (int i = 0; i < cameras.Length; i++) {
        //    RenderForCamera(spriteBatch, cameras.Buffer[i]);
        //}
        
        // render UI here? probably
    }

    void EnsureRenderGroupsSorted() {
        if (!renderGroupsNeedSorting) {
            return;
        }

        renderGroups.Sort();
        renderGroupsNeedSorting = false;
    }

    //void RenderForCamera(SpriteBatch spriteBatch, Camera camera) {
    //    RenderTarget2D mainTarget = camera.RenderTarget ?? sceneRenderTarget;
    //    bool mainTargetSet = false;
    //    bool mainTargetCleared = false;

    //    void SetMainTargetAsRenderTarget() {
    //        if (mainTargetSet) {
    //            return;
    //        }

    //        RDE.GraphicsDevice.SetRenderTarget(mainTarget);

    //        if (!mainTargetCleared) {
    //            RDE.GraphicsDevice.Clear(camera.ClearColor);
    //            mainTargetCleared = true;
    //        }

    //        mainTargetSet = true;
    //    }

    //    for (int i = 0; i < renderGroups.Length; i++) {
    //        RenderGroup renderGroup = renderGroups.Buffer[i];
    //        bool groupRendersToMainTarget = renderGroup.RenderTarget == null;

    //        if (groupRendersToMainTarget) {
    //            SetMainTargetAsRenderTarget();
    //        }

    //        renderGroup.Render(Renderables, spriteBatch, camera);

    //        if (!groupRendersToMainTarget) {
    //            // run through post processor(s) giving them a source and destination render target

    //            SetMainTargetAsRenderTarget();

    //            // draw destination render target
    //        }
    //    }

    //    if (mainTarget == camera.RenderTarget) {
    //        // run through post processor(s) giving them a source and destination render target
    //    }
    //}

    #region Lifecycle Methods

    internal void Load(BlueInstance instance) {
        BlueInstance = instance;
        Content = new ContentManager(instance.Services, instance.Content.RootDirectory);
        sceneRenderTarget = new RenderTarget2D(instance.GraphicsDevice, instance.GraphicsDevice.Viewport.Width, instance.GraphicsDevice.Viewport.Height);
        OnLoad();
    }

    /// <summary>
    /// Called when this scene is loaded. Only called once in the scene's lifetime.
    /// </summary>
    protected virtual void OnLoad() { }

    public void Unload() {
        for (int i = 0; i < Entities.Count; i++) {
            Entities[i].Destroy();
        }

        Content?.Dispose(); // This calls Content.Unload()
        sceneRenderTarget?.Dispose();

        OnUnload();

        BlueInstance = null;
    }

    /// <summary>
    /// Called when this scene is unloaded. Only called once in the scene's lifetime.
    /// </summary>
    protected virtual void OnUnload() { }

    #endregion

    #region Entity/Component Management

    /// <summary>
    /// Adds an <see cref="Entity"/> and all of its children entities to this scene.
    /// </summary>
    public void AddEntity(Entity entity) {
        if (entity == null || !Entities.Add(entity)) {
            return;
        }

        entity.Scene = this;
        entity.BlueInstance = BlueInstance;

        // add renderables

        for (int i = 0; i < entity.ChildCount; i++) {
            AddEntity(entity.GetChildAt(i));
        }
    }

    /// <summary>
    /// Removes an <see cref="Entity"/> and all of its children entities from the scene.
    /// </summary>
    /// <returns><see langword="true"/> if <paramref name="entity"/> was found and removed; otherwise <see langword="false"/>.</returns>
    public bool RemoveEntity(Entity entity) {
        if (entity == null || !Entities.Remove(entity)) {
            return false;
        }

        entity.Scene = null;
        entity.BlueInstance = null;

        // remove renderables

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
    /// <remarks>
    /// By default, this only searches for entities which are enabled in the hierarchy.
    /// Set <paramref name="includeDisabled"/> to <see langword="true"/> to include disabled entities in the search.
    /// </remarks>
    /// <param name="includeDisabled">(Optional) Include disabled entities in the search.</param>
    public void FindEntities<T>(List<T> results, bool includeDisabled = false) where T : Entity {
        Entities.FindAll(results, includeDisabled);
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for entities which are enabled in the hierarchy.
    /// Set <paramref name="includeDisabled"/> to <see langword="true"/> to include disabled entities in the search.
    /// </remarks>
    /// <param name="includeDisabled">(Optional) Include disabled entities in the search.</param>
    public T[] FindEntities<T>(bool includeDisabled = false) where T : Entity {
        return Entities.FindAll<T>(includeDisabled);
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
    /// <remarks>
    /// By default, this only searches for components which are enabled in the hierarchy.
    /// Set <paramref name="includeDisabled"/> to <see langword="true"/> to include disabled components in the search.
    /// </remarks>
    /// <param name="includeDisabled">(Optional) Include disabled components in the search.</param>
    public void FindComponents<T>(List<T> results, bool includeDisabled = false) where T : Component {
        Entities.FindComponents(results, includeDisabled);
    }

    /// <summary>
    /// Finds all components in this scene of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for components which are enabled in the hierarchy.
    /// Set <paramref name="includeDisabled"/> to <see langword="true"/> to include disabled components in the search.
    /// </remarks>
    /// <param name="includeDisabled">(Optional) Include disabled components in the search.</param>
    public T[] FindComponents<T>(bool includeDisabled = false) where T : Component {
        return Entities.FindComponents<T>(includeDisabled);
    }

    /// <summary>
    /// Adds a renderable to the scene.
    /// </summary>
    public void AddRenderable(IRenderable renderable) {
        renderables.Add(renderable);
    }

    /// <summary>
    /// Removes a renderable to the scene.
    /// </summary>
    public bool RemoveRenderable(IRenderable renderable) {
        return renderables.Remove(renderable);
    }

    #endregion
}
