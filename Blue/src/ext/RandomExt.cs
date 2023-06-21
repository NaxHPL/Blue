using System;

namespace BlueFw.Extensions;

public static class RandomExt {

    /// <summary>
    /// Returns a random float that is within a specified range.
    /// </summary>
    public static float Next(this Random random, float min, float max) {
        if (min >= max) {
            throw new ArgumentException("The minimum value must be less than the maximum!", nameof(min));
        }

        return random.NextSingle() * (max - min) + min;
    }

    /// <summary>
    /// Returns a random double that is within a specified range.
    /// </summary>
    public static double Next(this Random random, double min, double max) {
        if (min >= max) {
            throw new ArgumentException("The minimum value must be less than the maximum!", nameof(min));
        }

        return random.NextDouble() * (max - min) + min;
    }
}
