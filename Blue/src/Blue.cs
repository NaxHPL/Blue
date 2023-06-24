using BlueFw.Coroutines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BlueFw;

public class Blue : Game {

    /// <summary>
    /// Access to the <see cref="Blue"/> instance.
    /// </summary>
    public static Blue Instance => instance ?? throw new InvalidOperationException("Attempted to access the " + nameof(Blue) + " instance before it was created!");
    static Blue instance;

    /// <summary>
    /// The currently active scene.
    /// </summary>
    public Scene ActiveScene { get; private set; }

    /// <summary>
    /// Used to initialize and control the presentation of the graphics device.
    /// </summary>
    public GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Indicates if the game is the focused application.
    /// </summary>
    public bool IsGameInFocus => IsActive;

    /// <summary>
    /// Defines whether to update the game when it isn't the focused application.
    /// </summary>
    public bool PauseOnFocusLost = false;

    internal readonly CoroutineManager CoroutineManager = new CoroutineManager();

    SpriteBatch spriteBatch;
    Scene queuedSceneToLoad;

    public Blue() {
        if (instance != null) {
            throw new InvalidOperationException("Attempted to create a second instance of " + nameof(Blue) + "! Only one instance is allowed.");
        }
        
        instance = this;

        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        OnInitialize();
        base.Initialize();
    }

    /// <summary>
    /// Invoked when the instance is getting initialized.
    /// Use this to load non-graphical resources needed by the game and the first scene.
    /// </summary>
    protected virtual void OnInitialize() { }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        OnLoadContent();
    }

    /// <summary>
    /// Invoked when the instance is loading content.
    /// Use this to load any graphical resources needed by the game.
    /// </summary>
    protected virtual void OnLoadContent() { }

    protected override void Update(GameTime gameTime) {
        if ((PauseOnFocusLost && !IsActive) || gameTime.ElapsedGameTime.TotalSeconds <= 0d) {
            SuppressDraw();
            return;
        }

        Time.Update(gameTime);
        Input.Update();

        if (queuedSceneToLoad != null) {
            LoadSceneImmediate(queuedSceneToLoad);
            queuedSceneToLoad = null;
        }
        else {
            // We only want to update coroutines if we didn't load a scene this frame.
            // Coroutines which started when the new scene loaded will have already had their first iteration done.
            CoroutineManager.Update();
        }

        ActiveScene?.Update();

        BlueObject.DestroyQueuedObjects();
    }

    protected override void Draw(GameTime gameTime) {
        ActiveScene?.Render(spriteBatch);
    }

    /// <summary>
    /// Loads a scene. The scene will load at the start of the next frame.
    /// </summary>
    public void LoadScene(Scene scene) {
        ArgumentNullException.ThrowIfNull(scene, nameof(scene));

        if (scene == ActiveScene) {
            throw new ArgumentException("The scene you're trying to load is already active!", nameof(scene));
        }

        queuedSceneToLoad = scene;
    }

    /// <summary>
    /// Loads a scene of type <typeparamref name="T"/>. The scene will load at the start of the next frame.
    /// </summary>
    public T LoadScene<T>() where T : Scene, new() {
        T scene = new T();
        queuedSceneToLoad = scene;
        return scene;
    }

    void LoadSceneImmediate(Scene scene) {
        UnloadActiveScene();
        ActiveScene = scene; // Must get set as active scene before scene.Load()!
        scene.Load();
    }

    void UnloadActiveScene() {
        if (ActiveScene == null) {
            return;
        }

        ActiveScene.Unload();
        ActiveScene = null;
        GC.Collect();
    }

    protected override void OnExiting(object sender, EventArgs args) {
        UnloadActiveScene();
        base.OnExiting(sender, args);
    }
}
