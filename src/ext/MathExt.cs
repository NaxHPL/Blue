using Microsoft.Xna.Framework;

namespace Blue.src.ext;

public static class MathExt {

    /// <summary>
    /// Returns <paramref name="value"/> clamped to the range 0-1 inclusive.
    /// </summary>
    public static float Clamp01(float value) {
        return MathHelper.Clamp(value, 0f, 1f);
    }
}
