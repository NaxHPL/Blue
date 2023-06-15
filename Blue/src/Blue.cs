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
    /// Invoked when the instance is getting initialized.
    /// Use this event to load non-graphical resources needed by the game and the first scene.
    /// </summary>
    public event Action Initializing;

    /// <summary>
    /// Invoked when the instance is loading content.
    /// Use this event to load any graphical resources needed by the game.
    /// </summary>
    public event Action LoadingContent;

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
        Initializing?.Invoke();
        base.Initialize();
    }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        LoadingContent?.Invoke();
    }

    protected override void Update(GameTime gameTime) {
        if (PauseOnFocusLost && !IsActive) {
            SuppressDraw();
            return;
        }

        Time.Update(gameTime);
        Input.Update();

        if (queuedSceneToLoad != null) {
            LoadSceneImmediate(queuedSceneToLoad);
            queuedSceneToLoad = null;
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
