using Microsoft.Xna.Framework;
using System;

namespace Blue;

/// <summary>
/// Provides easy access to screen information and settings.
/// </summary>
public static class Screen {

    internal static BlueInstance BlueInstance;

    /// <summary>
    /// The width of the game's resolution.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static int Width {
        get => BlueInstance.Graphics.PreferredBackBufferWidth;
        set => BlueInstance.Graphics.PreferredBackBufferWidth = value;
    }

    /// <summary>
    /// The height of the game's resolution.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static int Height {
        get => BlueInstance.Graphics.PreferredBackBufferHeight;
        set => BlueInstance.Graphics.PreferredBackBufferHeight = value;
    }

    /// <summary>
    /// The width of the display displaying the game.
    /// </summary>
    public static int DisplayWidth => BlueInstance.GraphicsDevice.DisplayMode.Width;

    /// <summary>
    /// The height of the display displaying the game.
    /// </summary>
    public static int DisplayHeight => BlueInstance.GraphicsDevice.DisplayMode.Height;

    /// <summary>
    /// Idicates whether the game is running in fullscreen.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static bool IsFullscreen {
        get => BlueInstance.Graphics.IsFullScreen;
        set => BlueInstance.Graphics.IsFullScreen = value;
    }

    /// <summary>
    /// The system window that is presenting the game.
    /// </summary>
    public static GameWindow Window => BlueInstance.Window;

    /// <summary>
    /// The target frames per second when <see cref="IsFramerateCapped"/> is <see langword="true"/>.
    /// </summary>
    public static float TargetFramerate {
        get => MathF.Round(1f / (float)BlueInstance.TargetElapsedTime.TotalSeconds);
        set => BlueInstance.TargetElapsedTime = TimeSpan.FromSeconds(1d / value);
    }

    /// <summary>
    /// Indicates whether the framerate is capped. Use <see cref="TargetFramerate"/> to set the target frames per second.
    /// </summary>
    public static bool IsFramerateCapped {
        get => BlueInstance.IsFixedTimeStep;
        set => BlueInstance.IsFixedTimeStep = value;
    }

    /// <summary>
    /// Indicates whether or not to sync the game's frame rate with the monitor's refresh rate. <see cref="IsFramerateCapped"/> must be <see langword="false"/> for vsync to work.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static bool VsyncEnabled {
        get => BlueInstance.Graphics.SynchronizeWithVerticalRetrace;
        set => BlueInstance.Graphics.SynchronizeWithVerticalRetrace = value;
    }

    /// <summary>
    /// Apply any pending changes.
    /// </summary>
    public static void ApplyChanges() {
        BlueInstance.Graphics.ApplyChanges();
    }
}
