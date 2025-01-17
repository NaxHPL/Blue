using BlueFw.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BlueFw;

public class BlueSpriteBatch : SpriteBatch {

    Texture2D _whitePixel;
    Texture2D whitePixel {
        get {
            if (_whitePixel == null) {
                _whitePixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _whitePixel.SetData(new[] { Color.White });
                Disposing += (sender, args) => {
                    _whitePixel?.Dispose();
                    _whitePixel = null;
                };
            }

            return _whitePixel;
        }
    }

    Material currentMaterial;
    Matrix2D? currentTransformMatrix;

    internal BlueSpriteBatch(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }

    /// <summary>
    /// Begins a new batch with the specified material and transform matrix.
    /// </summary>
    /// <param name="transformMatrix">A matrix used to transform the sprite geometry.</param>
    internal void Begin(Material material, in Matrix2D? transformMatrix) {
        currentMaterial = material;
        currentTransformMatrix = transformMatrix;

        Begin(
            SpriteSortMode.Deferred,
            material.BlendState,
            material.SamplerState,
            material.DepthStencilState,
            material.RasterizerState,
            material.Effect,
            transformMatrix
        );
    }

    /// <summary>
    /// Rounds <paramref name="position"/> to the nearest integral values before drawing.
    /// </summary>
    public void Draw(
        Texture2D texture,
        Vector2 position,
        in Rectangle? sourceRectangle,
        in Color color,
        float rotation,
        in Vector2 origin,
        in Vector2 scale,
        SpriteEffects effects,
        bool roundPosition
    ) {
        if (roundPosition) {
            position.X = MathF.Round(position.X);
            position.Y = MathF.Round(position.Y);
        }

        Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, 0f);
    }

    /// <summary>
    /// Rounds <paramref name="position"/> to the nearest integral values before drawing.
    /// </summary>
    public void DrawString(
        SpriteFont spriteFont,
        string text,
        Vector2 position,
        in Color color,
        float rotation,
        in Vector2 origin,
        in Vector2 scale,
        SpriteEffects effects,
        bool roundPosition
    ) {
        if (roundPosition) {
            position.X = MathF.Round(position.X);
            position.Y = MathF.Round(position.Y);
        }

        DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, 0f, false);
    }

    /// <summary>
    /// Draws a filled in rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(in Rectangle rect, in Color color, bool roundPosition) {
        DrawRectangle(rect.Location.ToVector2(), rect.Size.ToVector2(), color, roundPosition);
    }

    /// <summary>
    /// Draws a filled in rectangle.
    /// </summary>
    /// <param name="x">The X coordinate of the rectangle's position.</param>
    /// <param name="y">The Y coordinate of the rectangle's position.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(float x, float y, float width, float height, in Color color, bool roundPosition) {
        DrawRectangle(new Vector2(x, y), new Vector2(width, height), color, roundPosition);
    }

    /// <summary>
    /// Draws a filled in rectangle.
    /// </summary>
    /// <param name="location">The location of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(in Vector2 location, in Vector2 size, in Color color, bool roundPosition) {
        Draw(whitePixel, location, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, roundPosition);
    }

    internal new void End() {
        base.End();

        currentMaterial = null;
        currentTransformMatrix = null;
    }

    /// <summary>
    /// Ends the current batch, then begins a new batch with the previously used material and transform matrix.
    /// </summary>
    internal void Flush() {
        Flush(currentMaterial, currentTransformMatrix);
    }

    /// <summary>
    /// Ends the current batch, then begins a new batch with the specified material and transform matrix.
    /// </summary>
    internal void Flush(Material material, in Matrix2D? transformMatrix) {
        base.End();
        Begin(material, transformMatrix);
    }

    /// <summary>
    /// Pauses the sprite batch so some custom rendering can be done.
    /// </summary>
    public void Pause() {
        base.End();
    }

    /// <summary>
    /// Resumes the sprite batch after custom rendering is finished.
    /// </summary>
    public void Resume() {
        Begin(currentMaterial, currentTransformMatrix);
    }
}