using BlueFw.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

public static class SpriteBatchExt {

    static Texture2D whitePixel;

    static Texture2D GetWhitePixel(SpriteBatch spriteBatch) {
        if (whitePixel == null) {
            whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            whitePixel.SetData(new[] { Color.White });
            spriteBatch.Disposing += (sender, args) => {
                whitePixel?.Dispose();
                whitePixel = null;
            };
        }

        return whitePixel;
    }

    /// <summary>
    /// Draws a filled in rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <param name="layerDepth">The depth of the layer of this rectangle.</param>
    public static void FillRectangle(this SpriteBatch spriteBatch, in Rectangle rect, in Color color, float layerDepth = 0f) {
        spriteBatch.FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, layerDepth);
    }

    /// <summary>
    /// Draws a filled in rectangle.
    /// </summary>
    /// <param name="x">The X coordinate of the rectangle's position.</param>
    /// <param name="y">The Y coordinate of the rectangle's position.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <param name="layerDepth">The depth of the layer of this rectangle.</param>
    public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, in Color color, float layerDepth = 0f) {
        spriteBatch.FillRectangle(new Vector2(x, y), new Vector2(width, height), color, layerDepth);
    }

    /// <summary>
    /// Draws a filled in rectangle.
    /// </summary>
    /// <param name="location">The location of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <param name="layerDepth">The depth of the layer of this rectangle.</param>
    public static void FillRectangle(this SpriteBatch spriteBatch, in Vector2 location, in Vector2 size, in Color color, float layerDepth = 0f) {
        spriteBatch.Draw(GetWhitePixel(spriteBatch), location, null, color, 0f, Vector2.Zero, size, SpriteEffects.None, layerDepth);
    }

    /// <summary>
    /// Begins a new batch with the specified material.
    /// </summary>
    public static void Begin(this SpriteBatch spriteBatch, Material material) {
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            material.BlendState,
            material.SamplerState,
            material.DepthStencilState,
            material.RasterizerState,
            material.Effect
        );
    }

    /// <summary>
    /// Begins a new batch with the specified material and transform matrix.
    /// </summary>
    /// <param name="transformMatrix">A matrix used to transform the sprite geometry.</param>
    public static void Begin(this SpriteBatch spriteBatch, Material material, in Matrix2D transformMatrix) {
        spriteBatch.Begin(
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
    /// Flushes the current batch, then begins a new batch with the specified material.
    /// </summary>
    public static void Flush(this SpriteBatch spriteBatch, Material material) {
        spriteBatch.End();
        spriteBatch.Begin(material);
    }

    /// <summary>
    /// Flushes the current batch, then begins a new batch with the specified material and transform matrix.
    /// </summary>
    /// <param name="transformMatrix">A matrix used to transform the sprite geometry.</param>
    public static void Flush(this SpriteBatch spriteBatch, Material material, in Matrix2D transformMatrix) {
        spriteBatch.End();
        spriteBatch.Begin(material, transformMatrix);
    }
}
