using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

internal static class NineSliceUtils {

    static Color[] srcColorData;
    static Color[] dstColorData;

    public static void GenerateScaledRects(Point desiredSize, Sprite sprite, NineSliceScaledRect[] rects) {
        if (rects.Length != 9) {
            throw new ArgumentException("The array must have a length of 9!", nameof(rects));
        }

        Point sprSize = sprite.Size;
        Rectangle nineSliceRect = sprite.NineSliceRect;
        int rightSideWidth = sprSize.X - nineSliceRect.Right;
        int bottomHeight = sprSize.Y - nineSliceRect.Bottom;

        // Set source rects
        rects[0].SourceRect = new Rectangle(0,                   0,                    nineSliceRect.X,     nineSliceRect.Y);
        rects[1].SourceRect = new Rectangle(nineSliceRect.X,     0,                    nineSliceRect.Width, nineSliceRect.Y);
        rects[2].SourceRect = new Rectangle(nineSliceRect.Right, 0,                    rightSideWidth,      nineSliceRect.Y);
        rects[3].SourceRect = new Rectangle(0,                   nineSliceRect.Y,      nineSliceRect.X,     nineSliceRect.Height);
        rects[4].SourceRect = new Rectangle(nineSliceRect.X,     nineSliceRect.Y,      nineSliceRect.Width, nineSliceRect.Height);
        rects[5].SourceRect = new Rectangle(nineSliceRect.Right, nineSliceRect.Y,      rightSideWidth,      nineSliceRect.Height);
        rects[6].SourceRect = new Rectangle(0,                   nineSliceRect.Bottom, nineSliceRect.X,     bottomHeight);
        rects[7].SourceRect = new Rectangle(nineSliceRect.X,     nineSliceRect.Bottom, nineSliceRect.Width, bottomHeight);
        rects[8].SourceRect = new Rectangle(nineSliceRect.Right, nineSliceRect.Bottom, rightSideWidth,      bottomHeight);

        if (sprite.SourceRect.HasValue) {
            for (int i = 0; i < rects.Length; i++) {
                rects[i].SourceRect.Offset(sprite.SourceRect.Value.Location);
            }
        }

        // Set scales
        int desiredMidWidth = desiredSize.X - nineSliceRect.X - rightSideWidth;
        int desiredMidHeight = desiredSize.Y - nineSliceRect.Y - bottomHeight;

        rects[0].Scale = new Vector2(1f,                                          1f);
        rects[1].Scale = new Vector2(rects[1].SourceRect.Width / desiredMidWidth, 1f);
        rects[2].Scale = new Vector2(1f,                                          1f);
        rects[3].Scale = new Vector2(1f,                                          rects[3].SourceRect.Height / desiredMidHeight);
        rects[4].Scale = new Vector2(rects[4].SourceRect.Width / desiredMidWidth, rects[4].SourceRect.Height / desiredMidHeight);
        rects[5].Scale = new Vector2(1f,                                          rects[5].SourceRect.Height / desiredMidHeight);
        rects[6].Scale = new Vector2(1f,                                          1f);
        rects[7].Scale = new Vector2(rects[7].SourceRect.Width / desiredMidWidth, 1f);
        rects[8].Scale = new Vector2(1f,                                          1f);

        // Set origins
        rects[0].Origin = new Vector2();
        rects[1].Origin = new Vector2();
        rects[2].Origin = new Vector2();
        rects[3].Origin = new Vector2();
        rects[4].Origin = new Vector2();
        rects[5].Origin = new Vector2();
        rects[6].Origin = new Vector2();
        rects[7].Origin = new Vector2();
        rects[8].Origin = new Vector2();
    }

    public static void GenerateTiledTexture(Sprite srcSprite, in Point desiredSize, Texture2D resultTex) {
        Point sprSize = srcSprite.Size;

        int srcColorDataSize = sprSize.X * sprSize.Y;
        PrepareSrcColorData(srcColorDataSize);

        int dstColorDataSize = desiredSize.X * desiredSize.Y;
        PrepareDstColorData(dstColorDataSize);

        srcSprite.Texture.GetData(0, srcSprite.SourceRect, srcColorData, 0, srcColorDataSize);

        // bottom right
        int cornerWidth = MathExt.Min(sprSize.X - srcSprite.NineSliceRect.Right, srcColorDataSize, dstColorDataSize);
        int cornerHeight = sprSize.Y - srcSprite.NineSliceRect.Bottom;
        int srcStartX = srcSprite.NineSliceRect.Right;
        int dstStartX = MathHelper.Max(0, desiredSize.X - (sprSize.X - srcSprite.NineSliceRect.Right));
        CopyBottomCorner(cornerWidth, cornerHeight, sprSize, desiredSize, srcStartX, dstStartX, srcColorDataSize, dstColorDataSize);

        // top right
        cornerHeight = srcSprite.NineSliceRect.Y;
        CopyTopCorner(cornerWidth, cornerHeight, sprSize.X, desiredSize.X, srcStartX, dstStartX, srcColorDataSize, dstColorDataSize);

        // bottom left
        cornerWidth = MathExt.Min(srcSprite.NineSliceRect.X, srcColorDataSize, dstColorDataSize);
        cornerHeight = sprSize.Y - srcSprite.NineSliceRect.Bottom;
        CopyBottomCorner(cornerWidth, cornerHeight, sprSize, desiredSize, 0, 0, srcColorDataSize, dstColorDataSize);

        // top left
        cornerHeight = srcSprite.NineSliceRect.Y;
        CopyTopCorner(cornerWidth, cornerHeight, sprSize.X, desiredSize.X, 0, 0, srcColorDataSize, dstColorDataSize);

        resultTex.SetData(dstColorData, 0, dstColorDataSize);
    }

    static void PrepareSrcColorData(int size) {
        if (srcColorData == null) {
            srcColorData = new Color[size];
            return;
        }

        if (srcColorData.Length < size) {
            Array.Resize(ref srcColorData, size);
        }

        Array.Fill(srcColorData, default, 0, size);
    }

    static void PrepareDstColorData(int size) {
        if (dstColorData == null) {
            dstColorData = new Color[size];
            return;
        }

        if (dstColorData.Length < size) {
            Array.Resize(ref dstColorData, size);
        }

        Array.Fill(dstColorData, default, 0, size);
    }

    static void CopyTopCorner(int cornerWidth,
                              int cornerHeight,
                              int spriteWidth,
                              int desiredWidth,
                              int srcStartX,
                              int dstStartX,
                              int srcColorDataSize,
                              int dstColorDataSize) {
        for (int y = 0; y < cornerHeight; y++) {
            int srcIndex = y * spriteWidth + srcStartX;
            int dstIndex = y * desiredWidth + dstStartX;

            srcIndex = MathHelper.Min(srcIndex, srcColorDataSize - cornerWidth);
            dstIndex = MathHelper.Min(dstIndex, dstColorDataSize - cornerWidth);

            Array.Copy(srcColorData, srcIndex, dstColorData, dstIndex, cornerWidth);
        }
    }

    static void CopyBottomCorner(int cornerWidth,
                                 int cornerHeight,
                                 in Point spriteSize,
                                 in Point desiredSize,
                                 int srcStartX,
                                 int dstStartX,
                                 int srcColorDataSize,
                                 int dstColorDataSize) {
        for (
            int srcY = spriteSize.Y - 1, dstY = desiredSize.Y - 1;
            srcY >= spriteSize.Y - cornerHeight && srcY >= 0 && dstY >= 0;
            srcY--, dstY--
        ) {
            int srcIndex = srcY * spriteSize.X + srcStartX;
            int dstIndex = dstY * desiredSize.X + dstStartX;

            srcIndex = MathHelper.Min(srcIndex, srcColorDataSize - cornerWidth);
            dstIndex = MathHelper.Min(dstIndex, dstColorDataSize - cornerWidth);

            Array.Copy(srcColorData, srcIndex, dstColorData, dstIndex, cornerWidth);
        }
    }
}
