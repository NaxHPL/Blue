using Microsoft.Xna.Framework;
using System;

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

    /// <summary>
    /// Returns the smallest integral value that is greater than or equal to <paramref name="value"/> as an integer.
    /// </summary>
    public static int CeilToInt(float value) {
        return (int)MathF.Ceiling(value);
    }

    /// <summary>
    /// Returns the largest integral value that is less than or equal to <paramref name="value"/> as an integer.
    /// </summary>
    public static int FloorToInt(float value) {
        return (int)MathF.Floor(value);
    }

    /// <summary>
    /// Maps a float from one range to another.
    /// </summary>
    public static float Map(this float number, float fromLower, float fromUpper, float toLower, float toUpper) {
        return (number - fromLower) / (fromUpper - fromLower) * (toUpper - toLower) + toLower;
    }
}
