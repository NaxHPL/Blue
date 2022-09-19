using Microsoft.Xna.Framework;
using System;

namespace BlueFw;

/// <summary>
/// Provides an interface to get time information for the current update cycle.
/// </summary>
public static class Time {

    /// <summary>
    /// Time in seconds since game startup.
    /// </summary>
    public static float TotalTime { get; private set; }

    /// <summary>
    /// Total number of frames that have passed.
    /// </summary>
    public static uint FrameCount { get; private set; }

    /// <summary>
    /// The interval in seconds from the last frame to the current one.
    /// </summary>
    public static float DeltaTime { get; private set; }

    /// <summary>
    /// The scale of <see cref="DeltaTime"/>.
    /// </summary>
    public static float TimeScale {
        get => timeScale;
        set => timeScale = MathHelper.Max(0f, value);
    }

    /// <summary>
    /// A secondary delta time useful for two simultaneous time scales.
    /// </summary>
    public static float AltDeltaTime { get; private set; }

    /// <summary>
    /// The scale of <see cref="AltDeltaTime"/>.
    /// </summary>
    public static float AltTimeScale {
        get => altTimeScale;
        set => altTimeScale = MathF.Max(0f, value);
    }

    /// <summary>
    /// The interval in seconds from the last frame to the current one. Not affected by <see cref="TimeScale"/>.
    /// </summary>
    public static float UnscaledDeltaTime { get; private set; }

    static float timeScale = 1f;
    static float altTimeScale = 1f;

    // This is called at the beginning of each Update
    internal static void Update(GameTime gameTime) {
        TotalTime = (float)gameTime.TotalGameTime.TotalSeconds;
        UnscaledDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        DeltaTime = UnscaledDeltaTime * timeScale;
        AltDeltaTime = UnscaledDeltaTime * altTimeScale;
        FrameCount++;
    }
}
