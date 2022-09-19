namespace BlueFw;

/// <summary>
/// The mode in which to play an animation.
/// </summary>
public enum PlayMode {

    /// <summary>
    /// When the sequence reaches its end, it will continue playing from the beginning.
    /// </summary>
    Loop,

    /// <summary>
    /// When the sequence reaches its end, it will reset to the beginning then stop.
    /// </summary>
    Once,

    /// <summary>
    /// When the sequence reaches its end, it will return to playing the previous sequence.
    /// </summary>
    OnceThenPrevious,

    /// <summary>
    /// When the sequence reaches its end, it will start playing in the reverse direction and ping pong back and forth.
    /// </summary>
    PingPong,

    /// <summary>
    /// When the sequence reaches its end, it will stop on the last frame (or first frame if playing in reverse).
    /// </summary>
    Clamp
}