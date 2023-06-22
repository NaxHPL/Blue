using BlueFw.Extensions;
using BlueFw.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

/// <summary>
/// Sprite text to be rendered in world space.
/// </summary>
public class SpriteText : SpriteTextBase, IRenderable { void IRenderable.Render(SpriteBatch spriteBatch, Camera camera) => Render(spriteBatch); }

/// <summary>
/// Sprite text to be rendered in screen space.
/// </summary>
public class UI_SpriteText : SpriteTextBase, IScreenRenderable { }

public class SpriteTextBase : Component {

    public int RenderLayer { get; set; }

    public float LayerDepth { get; set; }

    public Material Material { get; set; }

    public Rect Bounds {
        get { UpdateBounds(); return bounds; }
    }

    /// <summary>
    /// The text string to show.
    /// </summary>
    public string Text {
        get => text;
        set => SetText(value);
    }

    /// <summary>
    /// The font to use.
    /// </summary>
    public SpriteFont Font {
        get => font;
        set => SetFont(value);
    }

    /// <summary>
    /// Flip the text on the X axis?
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
    /// Flip the text on the Y axis?
    /// </summary>
    public bool FlipVertically {
        get => spriteEffects.HasFlag(SpriteEffects.FlipVertically);
        set {
            spriteEffects = value ?
                spriteEffects | SpriteEffects.FlipVertically :
                spriteEffects & ~SpriteEffects.FlipVertically;
        }
    }

    public Vector2 OriginNormalized {
        get => originNormalized;
        set => SetOriginNormalized(value);
    }

    /// <summary>
    /// A color to tint the text.
    /// </summary>
    public Color Tint = Color.White;

    /// <summary>
    /// Enable/disable a drop shadow effect.
    /// </summary>
    public bool DropShadowEnabled {
        get => dropShadowEnabled;
        set => SetDropShadowEnabled(value);
    }

    /// <summary>
    /// How much to offset the drop shadow by.
    /// </summary>
    public Vector2 DropShadowOffset {
        get => dropShadowOffset;
        set => SetDropShadowOffset(value);
    }

    /// <summary>
    /// A color to tint the drop shadow.
    /// </summary>
    public Color DropShadowTint = Color.Black;

    string text;
    SpriteFont font;
    SpriteEffects spriteEffects = SpriteEffects.None;
    bool dropShadowEnabled = false;
    Vector2 dropShadowOffset = Vector2.One;
    Vector2 transformedDropShadowOffset = Vector2.One; // this offset takes rotation into account
    Vector2 originNormalized;
    Vector2 origin;

    Rect bounds;
    bool boundsDirty;

    public void SetText(string text) {
        if (this.text == text) {
            return;
        }

        this.text = text;
        UpdateOrigin();
        boundsDirty = true;
    }

    public void SetFont(SpriteFont font) {
        if (this.font == font) {
            return;
        }

        this.font = font;
        UpdateOrigin();
        boundsDirty = true;
    }

    public void SetDropShadowEnabled(bool enabled) {
        if (dropShadowEnabled == enabled) {
            return;
        }

        dropShadowEnabled = enabled;

        if (Transform.Rotation != 0f) {
            UpdateTransformedDropShadowOffset();
        }
    }

    public void SetDropShadowOffset(Vector2 offset) {
        if (dropShadowOffset == offset) {
            return;
        }

        dropShadowOffset = offset;
        transformedDropShadowOffset = offset;

        if (Transform.Rotation != 0f) {
            UpdateTransformedDropShadowOffset();
        }
    }

    public void SetOriginNormalized(Vector2 originNormalized) {
        if (this.originNormalized == originNormalized) {
            return;
        }

        this.originNormalized = originNormalized;
        UpdateOrigin();
        boundsDirty = true;
    }

    void UpdateOrigin() {
        if (!string.IsNullOrEmpty(text) && font != null) {
            origin = originNormalized * font.MeasureString(text);
        }
    }

    protected override void OnEntityTransformChanged(Transform.ComponentFlags changedFlags) {
        boundsDirty = true;

        if (dropShadowEnabled && changedFlags.HasFlag(Transform.ComponentFlags.Rotation)) {
            UpdateTransformedDropShadowOffset();
        }
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        if (string.IsNullOrEmpty(text) || font == null) {
            bounds = Rect.Offscreen;
        }
        else {
            Vector2 position = Transform.Position - origin * Transform.Scale;
            Vector2 size = font.MeasureString(text) * Transform.Scale;

            if (dropShadowEnabled) {
                if (dropShadowOffset.X < 0f) {
                    position.X -= dropShadowOffset.X;
                }
                if (dropShadowOffset.Y < 0f) {
                    position.Y -= dropShadowOffset.Y;
                }

                size += Vector2Ext.Abs(dropShadowOffset) * Transform.Scale;
            }

            bounds.Position = position;
            bounds.Size = size;

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

    void UpdateTransformedDropShadowOffset() {
        Transform.TransformPoint(dropShadowOffset, out transformedDropShadowOffset);
    }

    public void Render(SpriteBatch spriteBatch) {
        if (text == null || font == null) {
            return;
        }

        if (dropShadowEnabled) {
            spriteBatch.DrawString(
                font,
                text,
                Transform.Position + transformedDropShadowOffset,
                DropShadowTint,
                Transform.Rotation,
                origin,
                Transform.Scale,
                spriteEffects,
                0f,
                false
            );
        }

        spriteBatch.DrawString(
            font,
            text,
            Transform.Position,
            Tint,
            Transform.Rotation,
            origin,
            Transform.Scale,
            spriteEffects,
            0f,
            false
        );
    }
}
