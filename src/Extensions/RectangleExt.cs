using Microsoft.Xna.Framework;

namespace BlueFw.Extensions;

public static class RectangleExt {

    /// <summary>
    /// Gets a <see cref="RectangleF"/> representation for this rectangle.
    /// </summary>
    public static RectangleF ToRect(this Rectangle rectangle) {
        RectangleF r;
        r.X = rectangle.X;
        r.Y = rectangle.Y;
        r.Width = rectangle.Width;
        r.Height = rectangle.Height;
        return r;
    }
}
