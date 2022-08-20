using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

internal static class NineSliceUtils {

    static Color[] srcColorData;
    static Color[] dstColorData;

    internal static void GenerateTexture(Sprite sourceSprite, Point desiredSize, Texture2D resultTex) {
        int srcColorDataSize = sourceSprite.Width * sourceSprite.Height;
        EnsureSrcColorDataSize(srcColorDataSize);
        
        int dstColorDataSize = desiredSize.X * desiredSize.Y;
        EnsureDstColorDataSize(dstColorDataSize);

        sourceSprite.Texture.GetData(0, sourceSprite.SourceRect, srcColorData, 0, srcColorDataSize);

        // do the stuff

        resultTex.SetData(dstColorData, 0, dstColorDataSize);
    }

    static void EnsureSrcColorDataSize(int size) {
        srcColorData ??= new Color[size];
        if (srcColorData.Length < size) {
            Array.Resize(ref srcColorData, size);
        }
    }

    static void EnsureDstColorDataSize(int size) {
        dstColorData ??= new Color[size];
        if (dstColorData.Length < size) {
            Array.Resize(ref dstColorData, size);
        }
    }
}
