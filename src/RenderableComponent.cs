using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

public abstract class RenderableComponent : Component, IRenderable {

    public bool Visible => visible && ActiveInHierarchy;

    public abstract RectangleF Bounds { get; }

    public int RenderLayer { get; set; }

    public float LayerDepth { get; set; }

    public Material Material { get; set; }

    bool visible;

    public bool CheckVisibleBy(Camera camera) {
        RectangleF.Overlaps(Bounds, camera.Bounds, out visible);
        return visible;
    }

    public abstract void Render(SpriteBatch spriteBatch);
}