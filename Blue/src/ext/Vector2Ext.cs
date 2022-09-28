﻿using Microsoft.Xna.Framework;
using System;

namespace BlueFw;

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
}