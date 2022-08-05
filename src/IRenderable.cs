using System.Collections.Generic;

namespace Blue;

internal interface IRenderable {

    /// <summary>
    /// Defines if this renderable will be rendered.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Gets whether this renderable is seen by the camera.
    /// </summary>
    bool Visible { get; }

    /// <summary>
    /// The world space bounds of this renderable.
    /// </summary>
    RectangleF Bounds { get; }

    /// <summary>
    /// The render layer this renderable is drawn on.
    /// </summary>
    int RenderLayer { get; }

    /// <summary>
    /// Defines when this renderable will be drawn within its render layer.
    /// </summary>
    float LayerOrder { get; }

    /// <summary>
    /// This renderable's material.
    /// </summary>
    Material Material { get; }

    /// <summary>
    /// Checks if this renderable is visible by <paramref name="camera"/>.
    /// </summary>
    /// <returns><see langword="true"/> if <paramref name="camera"/> can see this renderable; otherwise <see langword="false"/>.</returns>
    bool CheckVisibleBy(Camera camera);

    /// <summary>
    /// Renders this renderable using the specified sprite batch.
    /// </summary>
    /// <param name="spriteBatch"></param>
    void Render(SpriteBatch spriteBatch);
}

internal class RenderableOrderComparer : IComparer<IRenderable> {

    public int Compare(IRenderable a, IRenderable b) {
        int compareResult = a.RenderLayer.CompareTo(b.RenderLayer);
        if (compareResult != 0) {
            return compareResult;
        }

        compareResult = a.LayerOrder.CompareTo(b.LayerOrder);
        if (compareResult != 0) {
            return compareResult;
        }

        if (a.Material == b.Material) {
            return 0;
        }

        if (a.Material == null) {
            return -1;
        }

        return 1;
    }
}
