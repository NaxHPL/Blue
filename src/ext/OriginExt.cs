using Microsoft.Xna.Framework;

namespace BlueFw;

public static class OriginExt {

    /// <summary>
    /// Converts an <see cref="Origin"/> to a <see cref="Vector2"/> given a size.
    /// </summary>
    public static Vector2 ConvertToVector(this Origin origin, Point size) {
        return ConvertToVector(origin, size.X, size.Y);
    }

    /// <summary>
    /// Converts an <see cref="Origin"/> to a <see cref="Vector2"/> given a width and height.
    /// </summary>
    public static Vector2 ConvertToVector(this Origin origin, float width, float height) {
        return origin switch {
            Origin.Top         => new Vector2(width / 2f, 0f),
            Origin.TopRight    => new Vector2(width, 0f),
            Origin.Left        => new Vector2(0f, height / 2f),
            Origin.Center      => new Vector2(width / 2f, height / 2f),
            Origin.Right       => new Vector2(width, height / 2f),
            Origin.BottomLeft  => new Vector2(0f, height),
            Origin.Bottom      => new Vector2(width / 2f, height),
            Origin.BottomRight => new Vector2(width, height),
            _                  => new Vector2(0f, 0f)
        };
    }
}
