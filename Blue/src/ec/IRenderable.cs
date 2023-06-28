using BlueFw.Math;
using System.Collections.Generic;

namespace BlueFw;

public interface IRenderable {

    /// <summary>
    /// Defines if this renderable will be rendered.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// The render layer this renderable is drawn on.
    /// </summary>
    /// <remarks>
    /// If this value changes after this renderable is added to a scene, <see cref="Scene.RenderOrderDirty"/> must be called.
    /// </remarks>
    int RenderLayer { get; }

    /// <summary>
    /// Defines when this renderable will be drawn within its render layer.
    /// Larger depth values get rendered first.
    /// </summary>
    /// <remarks>
    /// If this value changes after this renderable is added to a scene, <see cref="Scene.RenderOrderDirty"/> must be called.
    /// </remarks>
    float LayerDepth { get; }

    /// <summary>
    /// This renderable's material.
    /// </summary>
    Material Material { get; }

    /// <summary>
    /// Defines if the renderable should be rendered in screen space or world space.
    /// </summary>
    bool RenderInScreenSpace { get; }

    /// <summary>
    /// Defines whether this renderable's position should be unrounded (<see langword="true"/>) or rounded to the nearest integral value (<see langword="false"/>).
    /// </summary>
    bool SubpixelPositioning { get; }

    /// <summary>
    /// The bounds of this renderable.
    /// </summary>
    Rect Bounds { get; }

    /// <summary>
    /// Renders this renderable using the specified sprite batch.
    /// </summary>
    void Render(BlueSpriteBatch spriteBatch, Camera camera);
}

internal class RenderableOrderComparer : IComparer<IRenderable> {

    public int Compare(IRenderable a, IRenderable b) {
        int compareResult = a.RenderLayer.CompareTo(b.RenderLayer);
        if (compareResult != 0) {
            return compareResult;
        }

        compareResult = b.LayerDepth.CompareTo(a.LayerDepth);
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
