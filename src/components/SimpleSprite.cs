using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

/// <summary>
/// A static sprite component.
/// </summary>
public class SimpleSprite : Component, IRenderable {

    public int RenderLayer { get; set; }

    public float LayerDepth { get; set; }

    public Material Material { get; set; }

    public Rect Bounds {
        get { UpdateBounds(); return bounds; }
    }

    /// <summary>
    /// The Sprite to draw.
    /// </summary>
    public Sprite Sprite {
        get => sprite;
        set => SetSprite(value);
    }

    /// <summary>
    /// Flip the sprite on the X axis?
    /// </summary>
    public bool FlipHorizontally {
        get => spriteEffects.HasFlag(SpriteEffects.FlipHorizontally);
        set {
            spriteEffects = value ?
                spriteEffects | SpriteEffects.FlipHorizontally :
                spriteEffects & ~SpriteEffects.FlipHorizontally;
        }
    }

    /// <summary>
    /// Flip the sprite on the Y axis?
    /// </summary>
    public bool FlipVertically {
        get => spriteEffects.HasFlag(SpriteEffects.FlipVertically);
        set {
            spriteEffects = value ?
                spriteEffects | SpriteEffects.FlipVertically :
                spriteEffects & ~SpriteEffects.FlipVertically;
        }
    }

    /// <summary>
    /// A color to tint the sprite.
    /// </summary>
    public Color Tint = Color.White;

    Rect bounds;
    Sprite sprite;
    SpriteEffects spriteEffects = SpriteEffects.None;

    bool boundsDirty;

    public void SetSprite(Sprite sprite) {
        if (this.sprite == sprite) {
            return;
        }

        this.sprite = sprite;
        boundsDirty = true;
    }

    protected override void OnEntityTransformChanged() {
        boundsDirty = true;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        if (sprite == null || sprite.Texture == null) {
            bounds = Rect.Offscreen;
        }
        else {
            Point size = sprite.SourceRect.HasValue ? sprite.SourceRect.Value.Size : sprite.Texture.Size();
            bounds.Size = size.ToVector2() * Transform.Scale;
            bounds.Position = Transform.Position - sprite.Origin * Transform.Scale;

            if (Transform.Rotation != 0f) {
                Vector2 topLeft = Transform.TransformPoint(new Vector2(bounds.X, bounds.Y));
                Vector2 topRight = Transform.TransformPoint(new Vector2(bounds.X + bounds.Width, bounds.Y));
                Vector2 bottomLeft = Transform.TransformPoint(new Vector2(bounds.X, bounds.Y + bounds.Height));
                Vector2 bottomRight = Transform.TransformPoint(new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height));

                float minX = MathExt.Min(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
                float maxX = MathExt.Max(topLeft.X, topRight.X, bottomLeft.X, bottomRight.X);
                float minY = MathExt.Min(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);
                float maxY = MathExt.Max(topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y);

                bounds.X = minX;
                bounds.Y = minY;
                bounds.Width = maxX - minX;
                bounds.Height = maxY - minY;
            }
        }

        boundsDirty = false;
    }

    public void Render(SpriteBatch spriteBatch, Camera camera) {
        if (sprite == null || sprite.Texture == null) {
            return;
        }

        spriteBatch.Draw(
            sprite.Texture,
            Transform.Position,
            sprite.SourceRect,
            Tint,
            Transform.Rotation,
            sprite.Origin,
            Transform.Scale,
            spriteEffects,
            0f
        );
    }

    protected override void OnDestroy() {
        sprite = null;
    }
}
