using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

public class Sprite {

    /// <summary>
    /// The Sprite's source texture.
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// The postiton and size of the Sprite on the texture.
    /// If <see langword="null"/> (default), the sprite is the whole texture.
    /// </summary>
    public Rectangle? SourceRect;

    /// <summary>
    /// The Sprite's origin (pivot point) specified in pixels. Default is the top left.
    /// </summary>
    public Vector2 Origin;

    /// <summary>
    /// Creates a Sprite
    /// </summary>
    public Sprite() { }

    /// <summary>
    /// Creates a Sprite with the specified texture, source rect, and origin.
    /// </summary>
    /// <param name="texture">The Sprite's source texture.</param>
    /// <param name="sourceRect">The postiton and size of the Sprite on the texture. If null, the sprite is the whole texture.</param>
    /// <param name="origin">The Sprite's origin (pivot point).</param>
    public Sprite(Texture2D texture, Rectangle? sourceRect, Vector2 origin) {
        Texture = texture;
        SourceRect = sourceRect;
        Origin = origin;
    }
}
