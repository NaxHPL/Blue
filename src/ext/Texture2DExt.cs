using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

public static class Texture2DExt {

    /// <summary>
    /// Gets the size of this texture as a <see cref="Point"/>.
    /// </summary>
    public static Point Size(this Texture2D texture) {
        return new Point(texture.Width, texture.Height);
    }
}
