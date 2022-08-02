using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BlueFw;

public class Blue : Game {

    /// <summary>
    /// Provides access to the Blue game instance.
    /// </summary>
    public static Blue Instance { get; private set; }

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
        if (Instance != null) {
            throw new Exception("You can only create one instance of " + nameof(Blue) + "!");
        }

        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
    }

    protected override void Initialize() {
        base.Initialize(); // Calls LoadContent()
    }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
        if (PauseOnFocusLost && !IsActive) {
            SuppressDraw();
            return;
        }

        Time.SetState(gameTime);
        Input.SetState(Mouse.GetState(), Keyboard.GetState());

        if (queuedSceneToLoad != null) {
            LoadSceneImmediate(queuedSceneToLoad);
            queuedSceneToLoad = null;
        }

        ActiveScene?.Update();
    }

    protected override void Draw(GameTime gameTime) {
        if (ActiveScene == null) {
            return;
        }

        ActiveScene.Render(spriteBatch);
    }

    /// <summary>
    /// Loads a scene. The scene will load at the start of the next frame.
    /// </summary>
    public void LoadScene(Scene scene) {
        if (scene == null) {
            throw new ArgumentNullException(nameof(scene), "The scene you're trying to load is null!");
        }

        if (scene == ActiveScene) {
            throw new ArgumentException("The scene you're trying to load is already active!", nameof(scene));
        }

        queuedSceneToLoad = scene;
    }

    void LoadSceneImmediate(Scene scene) {
        if (ActiveScene != null) {
            ActiveScene.Unload();
            ActiveScene = null;
            GC.Collect();
        }

        scene.Load();
        ActiveScene = scene;
    }
}
