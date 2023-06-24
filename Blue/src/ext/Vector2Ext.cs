using BlueFw.Math;
using Microsoft.Xna.Framework;
using System;

namespace BlueFw.Extensions;

public static class Vector2Ext {

    /// <summary>
    /// Returns a vector identical to <paramref name="vector"/> but with its length clamped to <paramref name="maxLength"/>.
    /// </summary>
    public static Vector2 ClampLength(in Vector2 vector, float maxLength) {
        if (vector == Vector2.Zero || vector.Length() <= maxLength) {
            return vector;
        }

        return Vector2.Normalize(vector) * maxLength;
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
        float x = vector.X * matrix.M11 + vector.Y * matrix.M21 + matrix.M31;
        float y = vector.X * matrix.M12 + vector.Y * matrix.M22 + matrix.M32;

        result.X = x;
        result.Y = y;
    }

    /// <summary>
    /// Returns a vector with the absolute value of <paramref name="vector"/>'s component values.
    /// </summary>
    public static Vector2 Abs(in Vector2 vector) {
        return new Vector2(MathF.Abs(vector.X), MathF.Abs(vector.Y));
    }

    public static Vector2 RotateAround(in Vector2 position, in Vector2 center, float radians) {
        RotateAround(position, center, radians, out Vector2 result);
        return result;
    }

    public static void RotateAround(in Vector2 position, in Vector2 center, float radians, out Vector2 result) {
        if (radians == 0f) {
            result = position;
        }
        else {
            // Translate to origin
            Vector2 translatedPos = position - center;

            // Do rotation
            float cosTheta = MathF.Cos(radians);
            float sinTheta = MathF.Sin(radians);

            result.X = translatedPos.X * cosTheta - translatedPos.Y * sinTheta;
            result.Y = translatedPos.X * sinTheta + translatedPos.Y * cosTheta;

            // Translate back to original position
            result += center;
        }
    }
}
