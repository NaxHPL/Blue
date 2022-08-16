using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

/// <summary>
/// A simple sprite.
/// </summary>
public class Sprite : Component, IRenderable {

    public int RenderLayer { get; set; }

    public float LayerDepth { get; set; }

    public Material Material { get; set; }

    public Rect Bounds {
        get { UpdateBounds(); return bounds; }
    }

    /// <summary>
    /// The texture to draw.
    /// </summary>
    public Texture2D Texture {
        get => texture;
        set => SetTexture(value);
    }

    /// <summary>
    /// The region of the texture that will be drawn.
    /// If <see langword="null"/> (default), the whole texture is drawn.
    /// </summary>
    public Rectangle? SourceRect {
        get => sourceRect;
        set => SetSourceRect(value);
    }

    /// <summary>
    /// The Sprite's origin (pivot point). Default is the top left.
    /// </summary>
    public Vector2 Origin {
        get => origin;
        set => SetOrigin(value);
    }

    /// <summary>
    /// The Sprite's origin (pivot point) normalized. Default is the top left.
    /// </summary>
    public Vector2 OriginNormalized {
        get {
            if (texture == null) {
                return Vector2.Zero;
            }

            return sourceRect.HasValue ?
                new Vector2(origin.X / sourceRect.Value.Width, origin.Y / sourceRect.Value.Height) :
                new Vector2(origin.X / texture.Width, origin.Y / texture.Height);
        }
        set {
            Vector2 newOrigin;

            if (texture == null) {
                newOrigin = Vector2.Zero;
            }
            else if (sourceRect.HasValue) {
                newOrigin.X = value.X * sourceRect.Value.Width;
                newOrigin.Y = value.Y * sourceRect.Value.Height;
            }
            else {
                newOrigin.X = value.X * texture.Width;
                newOrigin.Y = value.Y * texture.Height;
            }

            SetOrigin(newOrigin);
        }
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
    Texture2D texture;
    Rectangle? sourceRect;
    Vector2 origin;
    SpriteEffects spriteEffects = SpriteEffects.None;

    bool boundsDirty;

    /// <summary>
    /// Sets Sprite's the texture.
    /// </summary>
    public void SetTexture(Texture2D texture) {
        if (texture == this.texture) {
            return;
        }

        this.texture = texture;
        boundsDirty = true;
    }

    /// <summary>
    /// Sets the source rectangle.
    /// </summary>
    public void SetSourceRect(in Rectangle? sourceRect) {
        if (sourceRect == this.sourceRect) {
            return;
        }

        this.sourceRect = sourceRect;
        boundsDirty = true;
    }

    /// <summary>
    /// Sets the Sprite's origin (pivot point).
    /// </summary>
    public void SetOrigin(in Vector2 origin) {
        if (origin == this.origin) {
            return;
        }

        this.origin = origin;
        boundsDirty = true;
    }

    protected override void OnEntityTransformChanged() {
        boundsDirty = true;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        if (texture == null) {
            bounds = Rect.Zero;
            boundsDirty = false;
            return;
        }

        Point size = sourceRect.HasValue ? sourceRect.Value.Size : texture.Size();
        bounds.Size = size.ToVector2() * Transform.Scale;
        bounds.Position = Transform.Position - origin * Transform.Scale;

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

        boundsDirty = false;
    }

    public void Render(SpriteBatch spriteBatch, Camera camera) {
        if (Texture == null) {
            return;
        }

        spriteBatch.Draw(
            Texture,
            Transform.Position,
            SourceRect,
            Tint,
            Transform.Rotation,
            Origin,
            Transform.Scale,
            spriteEffects,
            0f
        );
    }
}
