using BlueFw.Math;
using Microsoft.Xna.Framework;

namespace BlueFw;

public static class RectangleExt {

    /// <summary>
    /// Gets a <see cref="Rect"/> representation for this rectangle.
    /// </summary>
    public static Rect ToRect(this Rectangle rectangle) {
        Rect r;
        r.X = rectangle.X;
        r.Y = rectangle.Y;
        r.Width = rectangle.Width;
        r.Height = rectangle.Height;
        return r;
    }
}
