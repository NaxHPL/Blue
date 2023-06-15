using BlueFw.Math;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BlueFw;

internal interface IScreenRenderable {

    /// <summary>
    /// Defines if this renderable will be rendered.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// The render layer this renderable is drawn on.
    /// </summary>
    /// <remarks>
    /// If you plan to change this value after this renderable is added to a scene,
    /// you must call <see cref="Scene.RenderOrderDirty"/> after changing it.
    /// </remarks>
    int RenderLayer { get; }

    /// <summary>
    /// Defines when this renderable will be drawn within its render layer.
    /// Larger depth values get rendered first.
    /// </summary>
    /// <remarks>
    /// If you plan to change this value after this renderable is added to a scene,
    /// you must call <see cref="Scene.RenderOrderDirty"/> after changing it.
    /// </remarks>
    float LayerDepth { get; }

    /// <summary>
    /// This renderable's material.
    /// </summary>
    Material Material { get; }

    /// <summary>
    /// The screen space bounds of this renderable.
    /// </summary>
    Rect Bounds { get; }

    /// <summary>
    /// Renders this renderable using the specified sprite batch.
    /// </summary>
    void Render(SpriteBatch spriteBatch);
}

internal class ScreenRenderableOrderComparer : IComparer<IScreenRenderable> {

    public int Compare(IScreenRenderable a, IScreenRenderable b) {
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
