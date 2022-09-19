using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

public static class Texture2DExt {

    /// <summary>
    /// Gets the size of this texture as a <see cref="Point"/>.
    /// </summary>
    public static Point Size(this Texture2D texture) {
        return new Point(texture.Width, texture.Height);
    }

    /// <summary>
    /// Slices this texture into Sprites.
    /// </summary>
    /// <param name="cellCount">The number of cells to split the texture into.</param>
    /// <param name="origin">The origin point for the Sprites.</param>
    /// <param name="textureRegion">Only split this region of the texture into Sprites. If <see langword="null"/>, the whole texture will be split.</param>
    public static Sprite[] SliceByCellCount(this Texture2D texture, in Point cellCount, Origin origin, Rectangle? textureRegion = null) {
        Point cellSize = new Point(
            (textureRegion.HasValue ? textureRegion.Value.Width : texture.Width) / cellCount.X,
            (textureRegion.HasValue ? textureRegion.Value.Height : texture.Height) / cellCount.Y
        );
        return SliceByCellSize(texture, cellSize, origin, textureRegion);
    }

    /// <summary>
    /// Slices this texture into Sprites.
    /// </summary>
    /// <param name="cellCount">The number of cells to split the texture into.</param>
    /// <param name="origin">The origin point for the Sprites.</param>
    /// <param name="textureRegion">Only split this region of the texture into Sprites. If <see langword="null"/>, the whole texture will be split.</param>
    public static Sprite[] SliceByCellCount(this Texture2D texture, in Point cellCount, in Vector2 origin, Rectangle? textureRegion = null) {
        Point cellSize = new Point(
            (textureRegion.HasValue ? textureRegion.Value.Width : texture.Width) / cellCount.X,
            (textureRegion.HasValue ? textureRegion.Value.Height : texture.Height) / cellCount.Y
        );
        return SliceByCellSize(texture, cellSize, origin, textureRegion);
    }

    /// <summary>
    /// Slices this texture into Sprites.
    /// </summary>
    /// <param name="cellSize">The size of each Sprite.</param>
    /// <param name="origin">The origin point for the Sprites.</param>
    /// <param name="textureRegion">Only split this region of the texture into Sprites. If <see langword="null"/>, the whole texture will be split.</param>
    public static Sprite[] SliceByCellSize(this Texture2D texture, in Point cellSize, Origin origin, Rectangle? textureRegion = null) {
        Vector2 originVec = origin.ConvertToVector(cellSize);
        return SliceByCellSize(texture, cellSize, originVec, textureRegion);
    }

    /// <summary>
    /// Slices this texture into Sprites.
    /// </summary>
    /// <param name="cellSize">The size of each Sprite.</param>
    /// <param name="origin">The origin point for the Sprites.</param>
    /// <param name="textureRegion">Only split this region of the texture into Sprites. If <see langword="null"/>, the whole texture will be split.</param>
    public static Sprite[] SliceByCellSize(this Texture2D texture, in Point cellSize, in Vector2 origin, Rectangle? textureRegion = null) {
        int numCellsX;
        int numCellsY;

        int sourceRectStartX;
        int sourceRectStartY;

        if (textureRegion.HasValue) {
            numCellsX = textureRegion.Value.Width / cellSize.X;
            numCellsY = textureRegion.Value.Height / cellSize.Y;

            sourceRectStartX = textureRegion.Value.X;
            sourceRectStartY = textureRegion.Value.Y;
        }
        else {
            numCellsX = texture.Width / cellSize.X;
            numCellsY = texture.Height / cellSize.Y;

            sourceRectStartX = 0;
            sourceRectStartY = 0;
        }

        Rectangle sourceRect;
        sourceRect.Width = cellSize.X;
        sourceRect.Height = cellSize.Y;

        Sprite[] sprites = new Sprite[numCellsX * numCellsY];

        int i = 0;
        for (int y = 0; y < numCellsY; y++) {
            sourceRect.Y = sourceRectStartY + y * cellSize.Y;
            for (int x = 0; x < numCellsX; x++) {
                sourceRect.X = sourceRectStartX + x * cellSize.X;
                sprites[i++] = new Sprite(texture, sourceRect, origin);
            }
        }

        return sprites;
    }
}
