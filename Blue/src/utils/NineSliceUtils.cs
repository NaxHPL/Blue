using BlueFw.Extensions;
using Microsoft.Xna.Framework;
using System;

namespace BlueFw.Utils;

internal static class NineSliceUtils {

    public static void GenerateScaledRects(in Point desiredSize, Sprite sprite, ref NineSlicePatch[] patches) {
        if (patches == null || patches.Length != 9) {
            Array.Resize(ref patches, 9);
        }

        Point sprSize = sprite.Size;
        Rectangle nsRect = sprite.NineSliceRect;
        int rightSideWidth = sprSize.X - nsRect.Right;
        int bottomHeight = sprSize.Y - nsRect.Bottom;

        // Set source rects
        patches[0].SourceRect = new Rectangle(0,            0,             nsRect.X,       nsRect.Y);
        patches[1].SourceRect = new Rectangle(nsRect.X,     0,             nsRect.Width,   nsRect.Y);
        patches[2].SourceRect = new Rectangle(nsRect.Right, 0,             rightSideWidth, nsRect.Y);
        patches[3].SourceRect = new Rectangle(0,            nsRect.Y,      nsRect.X,       nsRect.Height);
        patches[4].SourceRect = new Rectangle(nsRect.X,     nsRect.Y,      nsRect.Width,   nsRect.Height);
        patches[5].SourceRect = new Rectangle(nsRect.Right, nsRect.Y,      rightSideWidth, nsRect.Height);
        patches[6].SourceRect = new Rectangle(0,            nsRect.Bottom, nsRect.X,       bottomHeight);
        patches[7].SourceRect = new Rectangle(nsRect.X,     nsRect.Bottom, nsRect.Width,   bottomHeight);
        patches[8].SourceRect = new Rectangle(nsRect.Right, nsRect.Bottom, rightSideWidth, bottomHeight);

        // Set scales
        float desiredMidWidth = desiredSize.X - nsRect.X - rightSideWidth;
        float desiredMidHeight = desiredSize.Y - nsRect.Y - bottomHeight;

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

        patches[0].Origin = new Vector2(desiredOrigin.X,                                   desiredOrigin.Y);
        patches[1].Origin = new Vector2((desiredOrigin.X - nsRect.X) / patches[1].Scale.X, desiredOrigin.Y);
        patches[2].Origin = new Vector2(desiredOrigin.X - nsRect.X - desiredMidWidth,      desiredOrigin.Y);
        patches[3].Origin = new Vector2(desiredOrigin.X,                                   (desiredOrigin.Y - nsRect.Y) / patches[3].Scale.Y);
        patches[4].Origin = new Vector2(desiredOrigin.X - nsRect.X,                        desiredOrigin.Y - nsRect.Y) / patches[4].Scale;
        patches[5].Origin = new Vector2(desiredOrigin.X - nsRect.X - desiredMidWidth,      (desiredOrigin.Y - nsRect.Y) / patches[5].Scale.Y);
        patches[6].Origin = new Vector2(desiredOrigin.X,                                   desiredOrigin.Y - nsRect.Y - desiredMidHeight);
        patches[7].Origin = new Vector2((desiredOrigin.X - nsRect.X) / patches[7].Scale.X, desiredOrigin.Y - nsRect.Y - desiredMidHeight);
        patches[8].Origin = new Vector2(desiredOrigin.X - nsRect.X - desiredMidWidth,      desiredOrigin.Y - nsRect.Y - desiredMidHeight);

        // Offset to account for sprite source rect
        if (sprite.SourceRect.HasValue) {
            Point offset = sprite.SourceRect.Value.Location;
            for (int i = 0; i < patches.Length; i++) {
                patches[i].SourceRect.Offset(offset);
            }
        }
    }

    public static void GenerateTiledRects(in Point desiredSize, Sprite sprite, ref NineSlicePatch[] patches) {
        Point sprSize = sprite.Size;
        Rectangle nsRect = sprite.NineSliceRect;
        int rightSideWidth = sprSize.X - nsRect.Right;
        int bottomHeight = sprSize.Y - nsRect.Bottom;
        int desiredMidWidth = desiredSize.X - nsRect.X - rightSideWidth;
        int desiredMidHeight = desiredSize.Y - nsRect.Y - bottomHeight;

        const int numCornerPatches = 4;
        int numHorizPatches = MathExt.CeilToInt((float)desiredMidWidth / nsRect.Width);
        int numVertPatches = MathExt.CeilToInt((float)desiredMidHeight / nsRect.Height);
        int numCenterPatches = numHorizPatches * numVertPatches;
        int numTotalPatches = numCornerPatches + (numHorizPatches + numVertPatches) * 2 + numCenterPatches;

        if (patches == null || patches.Length != numTotalPatches) {
            Array.Resize(ref patches, numTotalPatches);
        }

        // Set source rects/origins
        Vector2 desiredOrigin = new Vector2(
            (sprite.Origin.X / sprite.Size.X) * desiredSize.X,
            (sprite.Origin.Y / sprite.Size.Y) * desiredSize.Y
        );

        int nextIdx = 0;

        // corners
        patches[nextIdx].SourceRect = new Rectangle(0, 0, nsRect.X, nsRect.Y);
        patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X, desiredOrigin.Y);
        patches[nextIdx].SourceRect = new Rectangle(nsRect.Right, 0, rightSideWidth, nsRect.Y);
        patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X - nsRect.X - desiredMidWidth, desiredOrigin.Y);
        patches[nextIdx].SourceRect = new Rectangle(0, nsRect.Bottom, nsRect.X, bottomHeight);
        patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X, desiredOrigin.Y - nsRect.Y - desiredMidHeight);
        patches[nextIdx].SourceRect = new Rectangle(nsRect.Right, nsRect.Bottom, rightSideWidth, bottomHeight);
        patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X - nsRect.X - desiredMidWidth, desiredOrigin.Y - nsRect.Y - desiredMidHeight);

        // top/bottom horizontals
        int currentX = nsRect.X;
        int widthRemaining = desiredMidWidth;
        while (widthRemaining > 0) {
            int width = widthRemaining >= nsRect.Width ? nsRect.Width : widthRemaining;

            patches[nextIdx].SourceRect = new Rectangle(nsRect.X, 0, width, nsRect.Y);
            patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X - currentX, desiredOrigin.Y);

            patches[nextIdx].SourceRect = new Rectangle(nsRect.X, nsRect.Bottom, width, bottomHeight);
            patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X - currentX, desiredOrigin.Y - nsRect.Y - desiredMidHeight);

            currentX += nsRect.Width;
            widthRemaining -= nsRect.Width;
        }

        // left/right verticals
        int currentY = nsRect.Y;
        int heightRemaining = desiredMidHeight;
        while (heightRemaining > 0) {
            int height = heightRemaining >= nsRect.Height ? nsRect.Height : heightRemaining;

            patches[nextIdx].SourceRect = new Rectangle(0, nsRect.Y, nsRect.X, height);
            patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X, desiredOrigin.Y - currentY);

            patches[nextIdx].SourceRect = new Rectangle(nsRect.Right, nsRect.Y, rightSideWidth, height);
            patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X - nsRect.X - desiredMidWidth, desiredOrigin.Y - currentY);

            currentY += nsRect.Height;
            heightRemaining -= nsRect.Height;
        }

        // centers
        currentY = nsRect.Y;
        heightRemaining = desiredMidHeight;
        while (heightRemaining > 0) {
            int height = heightRemaining >= nsRect.Height ? nsRect.Height : heightRemaining;

            currentX = nsRect.X;
            widthRemaining = desiredMidWidth;

            while (widthRemaining > 0) {
                int width = widthRemaining >= nsRect.Width ? nsRect.Width : widthRemaining;

                patches[nextIdx].SourceRect = new Rectangle(nsRect.X, nsRect.Y, width, height);
                patches[nextIdx++].Origin   = new Vector2(desiredOrigin.X - currentX, desiredOrigin.Y - currentY);

                currentX += nsRect.Width;
                widthRemaining -= nsRect.Width;
            }

            currentY += nsRect.Height;
            heightRemaining -= nsRect.Height;
        }

        // Set scales
        for (int i = 0; i < patches.Length; i++) {
            patches[i].Scale = Vector2.One;
        }

        // Offset to account for sprite source rect
        if (sprite.SourceRect.HasValue) {
            Point offset = sprite.SourceRect.Value.Location;
            for (int i = 0; i < patches.Length; i++) {
                patches[i].SourceRect.Offset(offset);
            }
        }
    }
}
