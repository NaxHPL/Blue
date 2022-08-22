using Microsoft.Xna.Framework;
using System;

namespace BlueFw;

internal static class NineSliceUtils {

    public static void GenerateScaledRects(Point desiredSize, Sprite sprite, ref NineSlicePatch[] patches) {
        patches ??= new NineSlicePatch[9];
        if (patches.Length != 9) {
            Array.Resize(ref patches, 9);
        }

        Point sprSize = sprite.Size;
        Rectangle nineSliceRect = sprite.NineSliceRect;
        int rightSideWidth = sprSize.X - nineSliceRect.Right;
        int bottomHeight = sprSize.Y - nineSliceRect.Bottom;

        // Set source rects
        patches[0].SourceRect = new Rectangle(0,                   0,                    nineSliceRect.X,     nineSliceRect.Y);
        patches[1].SourceRect = new Rectangle(nineSliceRect.X,     0,                    nineSliceRect.Width, nineSliceRect.Y);
        patches[2].SourceRect = new Rectangle(nineSliceRect.Right, 0,                    rightSideWidth,      nineSliceRect.Y);
        patches[3].SourceRect = new Rectangle(0,                   nineSliceRect.Y,      nineSliceRect.X,     nineSliceRect.Height);
        patches[4].SourceRect = new Rectangle(nineSliceRect.X,     nineSliceRect.Y,      nineSliceRect.Width, nineSliceRect.Height);
        patches[5].SourceRect = new Rectangle(nineSliceRect.Right, nineSliceRect.Y,      rightSideWidth,      nineSliceRect.Height);
        patches[6].SourceRect = new Rectangle(0,                   nineSliceRect.Bottom, nineSliceRect.X,     bottomHeight);
        patches[7].SourceRect = new Rectangle(nineSliceRect.X,     nineSliceRect.Bottom, nineSliceRect.Width, bottomHeight);
        patches[8].SourceRect = new Rectangle(nineSliceRect.Right, nineSliceRect.Bottom, rightSideWidth,      bottomHeight);

        // Set scales
        float desiredMidWidth = desiredSize.X - nineSliceRect.X - rightSideWidth;
        float desiredMidHeight = desiredSize.Y - nineSliceRect.Y - bottomHeight;

        patches[0].Scale = new Vector2(1f,                                            1f);
        patches[1].Scale = new Vector2(desiredMidWidth / patches[1].SourceRect.Width, 1f);
        patches[2].Scale = new Vector2(1f,                                            1f);
        patches[3].Scale = new Vector2(1f,                                            desiredMidHeight / patches[3].SourceRect.Height);
        patches[4].Scale = new Vector2(desiredMidWidth / patches[4].SourceRect.Width, desiredMidHeight / patches[4].SourceRect.Height);
        patches[5].Scale = new Vector2(1f,                                            desiredMidHeight / patches[5].SourceRect.Height);
        patches[6].Scale = new Vector2(1f,                                            1f);
        patches[7].Scale = new Vector2(desiredMidWidth / patches[7].SourceRect.Width, 1f);
        patches[8].Scale = new Vector2(1f,                                            1f);

        // Set origins
        Vector2 desiredOrigin = new Vector2(
            (sprite.Origin.X / sprite.Size.X) * desiredSize.X,
            (sprite.Origin.Y / sprite.Size.Y) * desiredSize.Y
        );

        patches[0].Origin = new Vector2(desiredOrigin.X, desiredOrigin.Y);
        patches[1].Origin = new Vector2((desiredOrigin.X - sprite.NineSliceRect.X) / patches[1].Scale.X, desiredOrigin.Y);
        patches[2].Origin = new Vector2(desiredOrigin.X - sprite.NineSliceRect.X - desiredMidWidth, desiredOrigin.Y);
        patches[3].Origin = new Vector2(desiredOrigin.X, (desiredOrigin.Y - sprite.NineSliceRect.Y) / patches[3].Scale.Y);
        patches[4].Origin = new Vector2(desiredOrigin.X - sprite.NineSliceRect.X, desiredOrigin.Y - sprite.NineSliceRect.Y) / patches[4].Scale;
        patches[5].Origin = new Vector2(desiredOrigin.X - sprite.NineSliceRect.X - desiredMidWidth, (desiredOrigin.Y - sprite.NineSliceRect.Y) / patches[5].Scale.Y);
        patches[6].Origin = new Vector2(desiredOrigin.X, desiredOrigin.Y - sprite.NineSliceRect.Y - desiredMidHeight);
        patches[7].Origin = new Vector2((desiredOrigin.X - sprite.NineSliceRect.X) / patches[1].Scale.X, desiredOrigin.Y - sprite.NineSliceRect.Y - desiredMidHeight);
        patches[8].Origin = new Vector2(desiredOrigin.X - sprite.NineSliceRect.X - desiredMidWidth, desiredOrigin.Y - sprite.NineSliceRect.Y - desiredMidHeight);

        // Offset to account for sprite source rect
        if (sprite.SourceRect.HasValue) {
            Point offset = sprite.SourceRect.Value.Location;
            for (int i = 0; i < patches.Length; i++) {
                patches[i].SourceRect.Offset(offset);
            }
        }
    }

    public static void GenerateTiledRects(Point desiredSize, Sprite sprite, ref NineSlicePatch[] patches) {

    }

    //static Color[] srcColorData;
    //static Color[] dstColorData;

    //public static void GenerateTiledTexture(Sprite srcSprite, in Point desiredSize, Texture2D resultTex) {
    //    Point sprSize = srcSprite.Size;

    //    int srcColorDataSize = sprSize.X * sprSize.Y;
    //    PrepareSrcColorData(srcColorDataSize);

    //    int dstColorDataSize = desiredSize.X * desiredSize.Y;
    //    PrepareDstColorData(dstColorDataSize);

    //    srcSprite.Texture.GetData(0, srcSprite.SourceRect, srcColorData, 0, srcColorDataSize);

    //    // bottom right
    //    int cornerWidth = MathExt.Min(sprSize.X - srcSprite.NineSliceRect.Right, srcColorDataSize, dstColorDataSize);
    //    int cornerHeight = sprSize.Y - srcSprite.NineSliceRect.Bottom;
    //    int srcStartX = srcSprite.NineSliceRect.Right;
    //    int dstStartX = MathHelper.Max(0, desiredSize.X - (sprSize.X - srcSprite.NineSliceRect.Right));
    //    CopyBottomCorner(cornerWidth, cornerHeight, sprSize, desiredSize, srcStartX, dstStartX, srcColorDataSize, dstColorDataSize);

    //    // top right
    //    cornerHeight = srcSprite.NineSliceRect.Y;
    //    CopyTopCorner(cornerWidth, cornerHeight, sprSize.X, desiredSize.X, srcStartX, dstStartX, srcColorDataSize, dstColorDataSize);

    //    // bottom left
    //    cornerWidth = MathExt.Min(srcSprite.NineSliceRect.X, srcColorDataSize, dstColorDataSize);
    //    cornerHeight = sprSize.Y - srcSprite.NineSliceRect.Bottom;
    //    CopyBottomCorner(cornerWidth, cornerHeight, sprSize, desiredSize, 0, 0, srcColorDataSize, dstColorDataSize);

    //    // top left
    //    cornerHeight = srcSprite.NineSliceRect.Y;
    //    CopyTopCorner(cornerWidth, cornerHeight, sprSize.X, desiredSize.X, 0, 0, srcColorDataSize, dstColorDataSize);

    //    resultTex.SetData(dstColorData, 0, dstColorDataSize);
    //}

    //static void PrepareSrcColorData(int size) {
    //    if (srcColorData == null) {
    //        srcColorData = new Color[size];
    //        return;
    //    }

    //    if (srcColorData.Length < size) {
    //        Array.Resize(ref srcColorData, size);
    //    }

    //    Array.Fill(srcColorData, default, 0, size);
    //}

    //static void PrepareDstColorData(int size) {
    //    if (dstColorData == null) {
    //        dstColorData = new Color[size];
    //        return;
    //    }

    //    if (dstColorData.Length < size) {
    //        Array.Resize(ref dstColorData, size);
    //    }

    //    Array.Fill(dstColorData, default, 0, size);
    //}

    //static void CopyTopCorner(int cornerWidth,
    //                          int cornerHeight,
    //                          int spriteWidth,
    //                          int desiredWidth,
    //                          int srcStartX,
    //                          int dstStartX,
    //                          int srcColorDataSize,
    //                          int dstColorDataSize) {
    //    for (int y = 0; y < cornerHeight; y++) {
    //        int srcIndex = y * spriteWidth + srcStartX;
    //        int dstIndex = y * desiredWidth + dstStartX;

    //        srcIndex = MathHelper.Min(srcIndex, srcColorDataSize - cornerWidth);
    //        dstIndex = MathHelper.Min(dstIndex, dstColorDataSize - cornerWidth);

    //        Array.Copy(srcColorData, srcIndex, dstColorData, dstIndex, cornerWidth);
    //    }
    //}

    //static void CopyBottomCorner(int cornerWidth,
    //                             int cornerHeight,
    //                             in Point spriteSize,
    //                             in Point desiredSize,
    //                             int srcStartX,
    //                             int dstStartX,
    //                             int srcColorDataSize,
    //                             int dstColorDataSize) {
    //    for (
    //        int srcY = spriteSize.Y - 1, dstY = desiredSize.Y - 1;
    //        srcY >= spriteSize.Y - cornerHeight && srcY >= 0 && dstY >= 0;
    //        srcY--, dstY--
    //    ) {
    //        int srcIndex = srcY * spriteSize.X + srcStartX;
    //        int dstIndex = dstY * desiredSize.X + dstStartX;

    //        srcIndex = MathHelper.Min(srcIndex, srcColorDataSize - cornerWidth);
    //        dstIndex = MathHelper.Min(dstIndex, dstColorDataSize - cornerWidth);

    //        Array.Copy(srcColorData, srcIndex, dstColorData, dstIndex, cornerWidth);
    //    }
    //}
}
