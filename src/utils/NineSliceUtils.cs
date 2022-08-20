using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

internal static class NineSliceUtils {

    static Color[] srcColorData;
    static Color[] dstColorData;

    internal static void GenerateTexture(Sprite srcSprite, in Point desiredSize, NineSliceMode mode, Texture2D resultTex) {
        Point sprSize = srcSprite.Size;

        int srcColorDataSize = sprSize.X * sprSize.Y;
        PrepareSrcColorData(srcColorDataSize);

        int dstColorDataSize = desiredSize.X * desiredSize.Y;
        EnsureDstColorDataSize(dstColorDataSize);

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
        for (int srcY = spriteSize.Y - 1, dstY = desiredSize.Y - 1; srcY >= spriteSize.Y - cornerHeight && srcY >= 0 && dstY >= 0; srcY--, dstY--) {
            int srcIndex = srcY * spriteSize.X + srcStartX;
            int dstIndex = dstY * desiredSize.X + dstStartX;

            srcIndex = MathHelper.Min(srcIndex, srcColorDataSize - cornerWidth);
            dstIndex = MathHelper.Min(dstIndex, dstColorDataSize - cornerWidth);

            Array.Copy(srcColorData, srcIndex, dstColorData, dstIndex, cornerWidth);
        }
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

    static void EnsureDstColorDataSize(int size) {
        if (dstColorData == null) {
            dstColorData = new Color[size];
            return;
        }

        if (dstColorData.Length < size) {
            Array.Resize(ref dstColorData, size);
        }

        Array.Fill(dstColorData, default, 0, size);
    }
}
