using Microsoft.Xna.Framework;

namespace BlueFw.Extensions;

public static class Vector2Ext {

    /// <summary>
    /// Returns a copy of this vector with its length clamped to <paramref name="maxLength"/>.
    /// </summary>
    public static Vector2 ClampLength(this Vector2 v, float maxLength) {
        if (v == Vector2.Zero || v.Length() <= maxLength) {
            return v;
        }

        return Vector2.Normalize(v) * maxLength;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a transformation of a 2D vector by a <see cref="Matrix2D"/>.
    /// </summary>
    public static Vector2 Transform(in Vector2 vector, in Matrix2D matrix) {
        Transform(vector, matrix, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Vector2"/> that contains a transformation of a 2D vector by a <see cref="Matrix2D"/> and stores it in <paramref name="result"/>.
    /// </summary>
    public static void Transform(in Vector2 vector, in Matrix2D matrix, out Vector2 result) {
        result.X = (vector.X * matrix.M11) + (vector.Y * matrix.M21) + matrix.M13;
        result.Y = (vector.X * matrix.M12) + (vector.Y * matrix.M22) + matrix.M23;
    }
}
