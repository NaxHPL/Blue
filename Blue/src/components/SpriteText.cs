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
public class SpriteTextUI : SpriteTextBase, IScreenRenderable { }

public abstract class SpriteTextBase : Component {

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
    Vector2 rotatedDropShadowOffset = Vector2.One;
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
        boundsDirty = true;
    }

    public void SetDropShadowOffset(Vector2 offset) {
        if (dropShadowOffset == offset) {
            return;
        }

        dropShadowOffset = offset;
        rotatedDropShadowOffset = offset;
        boundsDirty = true;
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
        if (font != null && !string.IsNullOrEmpty(text)) {
            origin = originNormalized * font.MeasureString(text);
        }
    }

    protected override void OnEntityTransformChanged(Transform.ComponentFlags changedFlags) {
        boundsDirty = true;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        if (font == null || Transform == null || string.IsNullOrEmpty(text)) {
            bounds = Rect.Offscreen;
        }
        else {
            Vector2 position = Transform.Position;
            Vector2 size = font.MeasureString(text);

            if (dropShadowEnabled) {
                position += Vector2.Min(Vector2.Zero, dropShadowOffset);
                size += Vector2Ext.Abs(dropShadowOffset);

                Vector2Ext.RotateAround(dropShadowOffset, Vector2.Zero, Transform.Rotation, out rotatedDropShadowOffset);
            }

            Rect.CalculateBounds(position, origin, size, Transform.Scale, Transform.Rotation, out bounds);
        }

        boundsDirty = false;
    }

    public void Render(SpriteBatch spriteBatch) {
        if (text == null || font == null) {
            return;
        }

        if (dropShadowEnabled) {
            spriteBatch.DrawString(
                font,
                text,
                Transform.Position + rotatedDropShadowOffset,
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
