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
    public bool DropShadowEnabled = false;

    /// <summary>
    /// How much to offset the drop shadow by.
    /// </summary>
    public Vector2 DropShadowOffset = Vector2.One;

    /// <summary>
    /// A color to tint the drop shadow.
    /// </summary>
    public Color DropShadowTint = Color.Black;

    string text;
    SpriteFont font;
    SpriteEffects spriteEffects = SpriteEffects.None;
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

    protected override void OnEntityTransformChanged() {
        boundsDirty = true;
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

            if (DropShadowEnabled) {
                if (DropShadowOffset.X < 0f) {
                    position.X -= DropShadowOffset.X;
                }
                if (DropShadowOffset.Y < 0f) {
                    position.Y -= DropShadowOffset.Y;
                }

                size += Vector2Ext.Abs(DropShadowOffset) * Transform.Scale;
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

    public void Render(SpriteBatch spriteBatch) {
        if (text == null || font == null) {
            return;
        }

        if (DropShadowEnabled) {
            spriteBatch.DrawString(
                font,
                text,
                Transform.Position + DropShadowOffset,
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
