using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BlueFw;

/// <summary>
/// A static sprite component.
/// </summary>
public class StaticSprite : Component, IRenderable {

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
    
    /// <summary>
    /// The nine slice mode to use when rendering this sprite. Default is <see cref="NineSliceMode.None"/>.
    /// </summary>
    public NineSliceMode NineSliceMode {
        get => nineSliceMode;
        set => SetNineSliceMode(value);
    }

    /// <summary>
    /// Get/set the size of the sprite when <see cref="NineSliceMode"/> is set to <see cref="NineSliceMode.Scale"/> or <see cref="NineSliceMode.Tile"/>.
    /// </summary>
    public Point Size {
        get => size;
        set => SetSize(value);
    }

    Sprite sprite;
    SpriteEffects spriteEffects = SpriteEffects.None;

    NineSliceMode nineSliceMode = NineSliceMode.None;
    Point size;
    NineSlicePatch[] scaledNineSlicePatches;
    NineSlicePatch[] tiledNineSlicePatches;

    Rect bounds;
    bool boundsDirty;

    /// <summary>
    /// Sets the sprite.
    /// </summary>
    public void SetSprite(Sprite sprite) {
        if (this.sprite == sprite) {
            return;
        }

        if (sprite.Texture == null) {
            throw new ArgumentException("The provided Sprite's texture is null!", nameof(sprite));
        }

        this.sprite = sprite;

        if (size == Point.Zero && sprite != null) {
            size = sprite.Size;
        }

        if (nineSliceMode == NineSliceMode.Scale && sprite != null) {
            NineSliceUtils.GenerateScaledRects(size, sprite, ref scaledNineSlicePatches);
        }
        else if (nineSliceMode == NineSliceMode.Tile && sprite != null) {
            NineSliceUtils.GenerateTiledRects(size, sprite, ref tiledNineSlicePatches);
        }

        boundsDirty = true;
    }

    /// <summary>
    /// Sets the size of the sprite when <see cref="NineSliceMode"/> is set to <see cref="NineSliceMode.Scale"/> or <see cref="NineSliceMode.Tile"/>.
    /// </summary>
    public void SetSize(Point size) {
        if (size.X < 1) {
            throw new ArgumentException("A sprite's width must be at least one!", nameof(size));
        }
        if (size.Y < 1) {
            throw new ArgumentException("A sprite's height must be at least one!", nameof(size));
        }

        if (this.size == size) {
            return;
        }

        this.size = size;

        if (nineSliceMode == NineSliceMode.Scale && sprite != null) {
            NineSliceUtils.GenerateScaledRects(size, sprite, ref scaledNineSlicePatches);
        }
        else if (nineSliceMode == NineSliceMode.Tile && sprite != null) {
            NineSliceUtils.GenerateTiledRects(size, sprite, ref tiledNineSlicePatches);
        }
        else {
            return;
        }

        boundsDirty = true;
    }

    public void SetNineSliceMode(NineSliceMode nineSliceMode) {
        if (this.nineSliceMode == nineSliceMode) {
            return;
        }

        this.nineSliceMode = nineSliceMode;

        if (nineSliceMode == NineSliceMode.Scale && sprite != null) {
            NineSliceUtils.GenerateScaledRects(size, sprite, ref scaledNineSlicePatches);
        }
        else if (nineSliceMode == NineSliceMode.Tile && sprite != null) {
            NineSliceUtils.GenerateTiledRects(size, sprite, ref tiledNineSlicePatches);
        }

        if (sprite != null && size != sprite.Size) {
            boundsDirty = true;
        }
    }

    protected override void OnEntityTransformChanged() {
        boundsDirty = true;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        if (sprite == null) {
            bounds = Rect.Offscreen;
        }
        else {
            Point size = nineSliceMode == NineSliceMode.None ? sprite.Size : this.size;
            bounds.Size = size.ToVector2() * Transform.Scale;

            Vector2 origin = nineSliceMode == NineSliceMode.None ?
                sprite.Origin :
                new Vector2(sprite.Origin.X / sprite.Size.X * size.X, sprite.Origin.Y / sprite.Size.Y * size.Y);
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
        }

        boundsDirty = false;
    }

    public void Render(SpriteBatch spriteBatch, Camera camera) {
        if (sprite == null) {
            return;
        }

        if (nineSliceMode == NineSliceMode.None) {
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
        else {
            NineSlicePatch[] patches = nineSliceMode == NineSliceMode.Scale ? scaledNineSlicePatches : tiledNineSlicePatches;
            for (int i = 0; i < patches.Length; i++) {
                spriteBatch.Draw(
                    sprite.Texture,
                    Transform.Position,
                    patches[i].SourceRect,
                    Tint,
                    Transform.Rotation,
                    patches[i].Origin,
                    patches[i].Scale * Transform.Scale,
                    spriteEffects,
                    0f
                );
            }
        }
    }

    protected override void OnDestroy() {
        sprite = null;
    }
}
