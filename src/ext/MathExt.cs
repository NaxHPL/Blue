﻿using Microsoft.Xna.Framework;

namespace BlueFw;

public static class MathExt {

    /// <summary>
    /// Returns <paramref name="value"/> clamped to the range 0-1 inclusive.
    /// </summary>
    public static float Clamp01(float value) {
        return MathHelper.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// Gets the minimum value of three floats.
    /// </summary>
    public static float Min(float value1, float value2, float value3) {
        return MathHelper.Min(value1, MathHelper.Min(value2, value3));
    }

    /// <summary>
    /// Gets the minimum value of four floats.
    /// </summary>
    public static float Min(float value1, float value2, float value3, float value4) {
        return MathHelper.Min(value1, MathHelper.Min(value2, MathHelper.Min(value3, value4)));
    }

    /// <summary>
    /// Gets the minimum value of three integers.
    /// </summary>
    public static int Min(int value1, int value2, int value3) {
        return MathHelper.Min(value1, MathHelper.Min(value2, value3));
    }

    /// <summary>
    /// Gets the minimum value of four integers.
    /// </summary>
    public static int Min(int value1, int value2, int value3, int value4) {
        return MathHelper.Min(value1, MathHelper.Min(value2, MathHelper.Min(value3, value4)));
    }

    /// <summary>
    /// Gets the maximum value of three floats.
    /// </summary>
    public static float Max(float value1, float value2, float value3) {
        return MathHelper.Max(value1, MathHelper.Max(value2, value3));
    }

    /// <summary>
    /// Gets the maximum value of four floats.
    /// </summary>
    public static float Max(float value1, float value2, float value3, float value4) {
        return MathHelper.Max(value1, MathHelper.Max(value2, MathHelper.Max(value3, value4)));
    }

    /// <summary>
    /// Gets the maximum value of three integers.
    /// </summary>
    public static int Max(int value1, int value2, int value3) {
        return MathHelper.Max(value1, MathHelper.Max(value2, value3));
    }

    /// <summary>
    /// Gets the maximum value of four integers.
    /// </summary>
    public static int Max(int value1, int value2, int value3, int value4) {
        return MathHelper.Max(value1, MathHelper.Max(value2, MathHelper.Max(value3, value4)));
    }
}
