using Microsoft.Xna.Framework;
using System;

namespace Blue;

/// <summary>
/// Provides easy access to screen information and settings.
/// </summary>
public class ScreenProperties {

    BlueInstance parentInstance;

    public ScreenProperties(BlueInstance parentInstance) {
        this.parentInstance = parentInstance;
    }

    /// <summary>
    /// The width of the game's resolution.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public int Width {
        get => parentInstance.Graphics.PreferredBackBufferWidth;
        set => parentInstance.Graphics.PreferredBackBufferWidth = value;
    }

    /// <summary>
    /// The height of the game's resolution.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public int Height {
        get => parentInstance.Graphics.PreferredBackBufferHeight;
        set => parentInstance.Graphics.PreferredBackBufferHeight = value;
    }

    /// <summary>
    /// The width of the display displaying the game.
    /// </summary>
    public int DisplayWidth => parentInstance.GraphicsDevice.DisplayMode.Width;

    /// <summary>
    /// The height of the display displaying the game.
    /// </summary>
    public int DisplayHeight => parentInstance.GraphicsDevice.DisplayMode.Height;

    /// <summary>
    /// Idicates whether the game is running in fullscreen.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public bool IsFullscreen {
        get => parentInstance.Graphics.IsFullScreen;
        set => parentInstance.Graphics.IsFullScreen = value;
    }

    /// <summary>
    /// The system window that is presenting the game.
    /// </summary>
    public GameWindow Window => parentInstance.Window;

    /// <summary>
    /// The target frames per second when <see cref="IsFramerateCapped"/> is <see langword="true"/>.
    /// </summary>
    public float TargetFramerate {
        get => MathF.Round(1f / (float)parentInstance.TargetElapsedTime.TotalSeconds);
        set => parentInstance.TargetElapsedTime = TimeSpan.FromSeconds(1d / value);
    }

    /// <summary>
    /// Indicates whether the framerate is capped. Use <see cref="TargetFramerate"/> to set the target frames per second.
    /// </summary>
    public bool IsFramerateCapped {
        get => parentInstance.IsFixedTimeStep;
        set => parentInstance.IsFixedTimeStep = value;
    }

    /// <summary>
    /// Indicates whether or not to sync the game's frame rate with the monitor's refresh rate. <see cref="IsFramerateCapped"/> must be <see langword="false"/> for vsync to work.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public bool VsyncEnabled {
        get => parentInstance.Graphics.SynchronizeWithVerticalRetrace;
        set => parentInstance.Graphics.SynchronizeWithVerticalRetrace = value;
    }

    /// <summary>
    /// Apply any pending changes.
    /// </summary>
    public void ApplyChanges() {
        parentInstance.Graphics.ApplyChanges();
    }
}
