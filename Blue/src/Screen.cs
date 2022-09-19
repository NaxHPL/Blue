using Microsoft.Xna.Framework;
using System;

namespace BlueFw;

/// <summary>
/// Provides easy access to screen information and settings.
/// </summary>
public static class Screen {

    /// <summary>
    /// The width of the game's resolution.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static int Width {
        get => Blue.Instance.Graphics.PreferredBackBufferWidth;
        set => Blue.Instance.Graphics.PreferredBackBufferWidth = value;
    }

    /// <summary>
    /// The height of the game's resolution.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static int Height {
        get => Blue.Instance.Graphics.PreferredBackBufferHeight;
        set => Blue.Instance.Graphics.PreferredBackBufferHeight = value;
    }

    /// <summary>
    /// The width of the display displaying the game.
    /// </summary>
    public static int DisplayWidth => Blue.Instance.GraphicsDevice.DisplayMode.Width;

    /// <summary>
    /// The height of the display displaying the game.
    /// </summary>
    public static int DisplayHeight => Blue.Instance.GraphicsDevice.DisplayMode.Height;

    /// <summary>
    /// Idicates whether the game is running in fullscreen.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static bool IsFullscreen {
        get => Blue.Instance.Graphics.IsFullScreen;
        set => Blue.Instance.Graphics.IsFullScreen = value;
    }

    /// <summary>
    /// The system window that is presenting the game.
    /// </summary>
    public static GameWindow Window => Blue.Instance.Window;

    /// <summary>
    /// The target frames per second when <see cref="IsFramerateCapped"/> is <see langword="true"/>.
    /// </summary>
    public static float TargetFramerate {
        get => MathF.Round(1f / (float)Blue.Instance.TargetElapsedTime.TotalSeconds);
        set => Blue.Instance.TargetElapsedTime = TimeSpan.FromSeconds(1d / value);
    }

    /// <summary>
    /// Indicates whether the framerate is capped. Use <see cref="TargetFramerate"/> to set the target frames per second.
    /// </summary>
    public static bool IsFramerateCapped {
        get => Blue.Instance.IsFixedTimeStep;
        set => Blue.Instance.IsFixedTimeStep = value;
    }

    /// <summary>
    /// Indicates whether or not to sync the game's frame rate with the monitor's refresh rate. <see cref="IsFramerateCapped"/> must be <see langword="false"/> for vsync to work.
    /// </summary>
    /// <remarks>When setting this value, you must call <see cref="ApplyChanges"/> to apply the change.</remarks>
    public static bool VsyncEnabled {
        get => Blue.Instance.Graphics.SynchronizeWithVerticalRetrace;
        set => Blue.Instance.Graphics.SynchronizeWithVerticalRetrace = value;
    }

    /// <summary>
    /// Apply any pending changes.
    /// </summary>
    public static void ApplyChanges() {
        Blue.Instance.Graphics.ApplyChanges();
    }
}
