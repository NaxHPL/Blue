﻿using Microsoft.Xna.Framework;
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
    Texture2D nineSliceTex;
    Point size;
    Vector2 nineSliceOrigin;

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

        if (sprite != null) {
            size = sprite.Size;
        }

        if (nineSliceMode != NineSliceMode.None) {
            GenerateNineSliceTexture();
        }

        boundsDirty = true;
    }

    /// <summary>
    /// Sets the size of the sprite when <see cref="NineSliceMode"/> is set to <see cref="NineSliceMode.Scale"/> or <see cref="NineSliceMode.Tile"/>.
    /// </summary>
    public void SetSize(Point size) {
        if (this.size == size) {
            return;
        }

        this.size = size;

        if (nineSliceMode != NineSliceMode.None) {
            GenerateNineSliceTexture();
            boundsDirty = true;
        }
    }

    public void SetNineSliceMode(NineSliceMode nineSliceMode) {
        if (this.nineSliceMode == nineSliceMode) {
            return;
        }

        this.nineSliceMode = nineSliceMode;

        if (nineSliceMode != NineSliceMode.None) {
            GenerateNineSliceTexture();
        }

        if (sprite != null && size != sprite.Texture.Size()) {
            boundsDirty = true;
        }
    }

    void GenerateNineSliceTexture() {
        if (sprite == null) {
            return;
        }

        // Prepare texture
        if (nineSliceTex != null && nineSliceTex.Size() != size) {
            nineSliceTex.Dispose();
            nineSliceTex = null;
        }
        nineSliceTex ??= new Texture2D(Blue.Instance.GraphicsDevice, size.X, size.Y);

        NineSliceUtils.GenerateTexture(sprite, size, nineSliceTex);
        UpdateNineSliceOrigin();
    }

    void UpdateNineSliceOrigin() {
        if (sprite == null) {
            return;
        }

        float sprOriginNormalizedX = sprite.Origin.X / sprite.Size.X;
        float sprOriginNormalizedY = sprite.Origin.Y / sprite.Size.Y;

        nineSliceOrigin.X = sprOriginNormalizedX * size.X;
        nineSliceOrigin.Y = sprOriginNormalizedY * size.Y;
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

            Vector2 origin = nineSliceMode == NineSliceMode.None ? sprite.Origin : nineSliceOrigin;
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
            spriteBatch.Draw(
                nineSliceTex,
                Transform.Position,
                null,
                Tint,
                Transform.Rotation,
                nineSliceOrigin,
                Transform.Scale,
                spriteEffects,
                0f
            );
        }
    }

    protected override void OnDestroy() {
        sprite = null;

        if (nineSliceTex != null) {
            nineSliceTex.Dispose();
            nineSliceTex = null;
        }
    }
}
