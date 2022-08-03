using Microsoft.Xna.Framework;
using System;

namespace Blue.Math;

/// <summary>
/// Represents a 2D transformation matrix.
/// </summary>
public struct Matrix2D : IEquatable<Matrix2D> {

    #region Public Properties

    /// <summary>
    /// The identity matrix.
    /// </summary>
    public static Matrix2D Identity => identity;

    static Matrix2D identity = new Matrix2D(
        1f, 0f, 0f,
        0f, 1f, 0f
    );

    /// <summary>
    /// The determinant of this matrix.
    /// </summary>
    public float Determinant => M11 * M22 - M12 * M21;

    /// <summary>
    /// The translation stored in this matrix.
    /// </summary>
    public Vector2 Translation {
        get => new Vector2(M13, M23);
        set {
            M13 = value.X;
            M23 = value.Y;
        }
    }

    /// <summary>
    /// The scale stored in this matrix.
    /// </summary>
    public Vector2 Scale {
        get => new Vector2(M11, M22);
        set {
            M11 = value.X;
            M22 = value.Y;
        }
    }

    /// <summary>
    /// The rotation stored in this matrix in radians.
    /// </summary>
    public float Rotation {
        get => MathF.Atan2(M21, M11);
        set {
            float cos = MathF.Cos(value);
            float sin = MathF.Sin(value);

            M11 = cos;
            M12 = sin;
            M21 = -sin;
            M22 = cos;
        }
    }

    /// <summary>
    /// The translation stored in this matrix in degrees.
    /// </summary>
    public float RotationDegress {
        get => MathHelper.ToDegrees(Rotation);
        set => Rotation = MathHelper.ToRadians(value);
    }

    #endregion

    public float M11, M12, M13;
    public float M21, M22, M23;

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> with the specified values.
    /// </summary>
    public Matrix2D(float m11, float m12, float m13,
                    float m21, float m22, float m23) {
        M11 = m11; M12 = m12; M13 = m13;
        M21 = m21; M22 = m22; M23 = m23;
    }

    #region Translation

    /// <summary>
    /// Creates a new translation <see cref="Matrix2D"/> with the specified position.
    /// </summary>
    /// <param name="pos">X and Y coordinates of the translation.</param>
    public static Matrix2D CreateTranslation(in Vector2 pos) {
        CreateTranslation(pos.X, pos.Y, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a translation <see cref="Matrix2D"/> with the specified position and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="pos">X and Y coordinates of the translation.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateTranslation(in Vector2 pos, out Matrix2D result) {
        CreateTranslation(pos.X, pos.Y, out result);
    }

    /// <summary>
    /// Creates a new translation <see cref="Matrix2D"/> with the specified X and Y position.
    /// </summary>
    /// <param name="posX">X coordinate of the translation.</param>
    /// <param name="posY">Y coordinate of the translation.</param>
    public static Matrix2D CreateTranslation(float posX, float posY) {
        CreateTranslation(posX, posY, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a translation <see cref="Matrix2D"/> with the specified X and Y position and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="posX">X coordinate of the translation.</param>
    /// <param name="posY">Y coordinate of the translation.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateTranslation(float posX, float posY, out Matrix2D result) {
        result.M11 = 1f;
        result.M12 = 0f;
        result.M13 = posX;

        result.M21 = 0f;
        result.M22 = 1f;
        result.M23 = posY;
    }

    #endregion

    #region Scale

    /// <summary>
    /// Creates a new scale <see cref="Matrix2D"/> with the specified scale applied to both axes.
    /// </summary>
    /// <param name="scale">The scale value for both axes.</param>
    public static Matrix2D CreateScale(float scale) {
        CreateScale(scale, scale, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a scale <see cref="Matrix2D"/> with the specified scale applied to both axes and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="scale">The scale value for both axes.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateScale(float scale, out Matrix2D result) {
        CreateScale(scale, scale, out result);
    }

    /// <summary>
    /// Creates a new scale <see cref="Matrix2D"/> with the specified scale.
    /// </summary>
    /// <param name="scale">The X and Y scale values.</param>
    public static Matrix2D CreateScale(in Vector2 scale) {
        CreateScale(scale.X, scale.Y, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a scale <see cref="Matrix2D"/> with the specified scale and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="scale">The X and Y scale values.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateScale(in Vector2 scale, out Matrix2D result) {
        CreateScale(scale.X, scale.Y, out result);
    }

    /// <summary>
    /// Creates a new scale <see cref="Matrix2D"/> with the specified X and Y scale values.
    /// </summary>
    /// <param name="scaleX">The X axis scale value.</param>
    /// <param name="scaleY">The Y axis scale value.</param>
    public static Matrix2D CreateScale(float scaleX, float scaleY) {
        CreateScale(scaleX, scaleY, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a scale <see cref="Matrix2D"/> with the specified X and Y scale values and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="scaleX">The X axis scale value.</param>
    /// <param name="scaleY">The Y axis scale value.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateScale(float scaleX, float scaleY, out Matrix2D result) {
        result.M11 = scaleX;
        result.M12 = 0f;
        result.M13 = 0f;

        result.M21 = 0f;
        result.M22 = scaleY;
        result.M23 = 0f;
    }

    #endregion

    #region Rotation

    /// <summary>
    /// Creates a new rotation <see cref="Matrix2D"/> with the specified angle (degrees).
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    public static Matrix2D CreateRotationDegress(float degrees) {
        CreateRotation(MathHelper.ToRadians(degrees), out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a rotation <see cref="Matrix2D"/> with the specified angle (degrees) and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="radians">The angle in degrees.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateRotationDegrees(float degrees, out Matrix2D result) {
        CreateRotation(MathHelper.ToRadians(degrees), out result);
    }

    /// <summary>
    /// Creates a new rotation <see cref="Matrix2D"/> with the specified angle (radians).
    /// </summary>
    /// <param name="radians">The angle in radians.</param>
    public static Matrix2D CreateRotation(float radians) {
        CreateRotation(radians, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a rotation <see cref="Matrix2D"/> with the specified angle (radians) and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="radians">The angle in radians.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void CreateRotation(float radians, out Matrix2D result) {
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);

        result.M11 = cos;
        result.M12 = sin;
        result.M13 = 0f;

        result.M21 = -sin;
        result.M22 = cos;
        result.M23 = 0f;
    }

    #endregion

    #region Math Operators

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the sum of two matrices.
    /// </summary>
    public static Matrix2D Add(in Matrix2D matrix1, in Matrix2D matrix2) {
        Add(matrix1, matrix2, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the sum of two matrices and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Add(in Matrix2D matrix1, in Matrix2D matrix2, out Matrix2D result) {
        result.M11 = matrix1.M11 + matrix2.M11;
        result.M12 = matrix1.M12 + matrix2.M12;
        result.M13 = matrix1.M13 + matrix2.M13;

        result.M21 = matrix1.M21 + matrix2.M21;
        result.M22 = matrix1.M22 + matrix2.M22;
        result.M23 = matrix1.M23 + matrix2.M23;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the difference of two matrices.
    /// </summary>
    /// <param name="matrix1">The left <see cref="Matrix2D"/>.</param>
    /// <param name="matrix2">The right <see cref="Matrix2D"/>.</param>
    public static Matrix2D Subtract(in Matrix2D matrix1, in Matrix2D matrix2) {
        Subtract(matrix1, matrix2, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the difference of two matrices and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="matrix1">The left <see cref="Matrix2D"/>.</param>
    /// <param name="matrix2">The right <see cref="Matrix2D"/>.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Subtract(in Matrix2D matrix1, in Matrix2D matrix2, out Matrix2D result) {
        result.M11 = matrix1.M11 - matrix2.M11;
        result.M12 = matrix1.M12 - matrix2.M12;
        result.M13 = matrix1.M13 - matrix2.M13;

        result.M21 = matrix1.M21 - matrix2.M21;
        result.M22 = matrix1.M22 - matrix2.M22;
        result.M23 = matrix1.M23 - matrix2.M23;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the multiplication of a <see cref="Matrix2D"/> and a scalar.
    /// </summary>
    public static Matrix2D Multiply(in Matrix2D matrix, float scalar) {
        Multiply(matrix, scalar, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the multiplication of a <see cref="Matrix2D"/> and a scalar and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Multiply(in Matrix2D matrix, float scalar, out Matrix2D result) {
        result.M11 = matrix.M11 * scalar;
        result.M12 = matrix.M12 * scalar;
        result.M13 = matrix.M13 * scalar;

        result.M21 = matrix.M21 * scalar;
        result.M22 = matrix.M22 * scalar;
        result.M23 = matrix.M23 * scalar;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the multiplication of two matrices.
    /// </summary>
    /// <param name="matrix1">The left <see cref="Matrix2D"/>.</param>
    /// <param name="matrix2">The right <see cref="Matrix2D"/>.</param>
    public static Matrix2D Multiply(in Matrix2D matrix1, in Matrix2D matrix2) {
        Multiply(matrix1, matrix2, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the multiplication of two matrices and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="matrix1">The left <see cref="Matrix2D"/>.</param>
    /// <param name="matrix2">The right <see cref="Matrix2D"/>.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Multiply(in Matrix2D matrix1, in Matrix2D matrix2, out Matrix2D result) {
        result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21;
        result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22;
        result.M13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M21 + matrix2.M13;

        result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21;
        result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22;
        result.M23 = matrix1.M13 * matrix2.M12 + matrix1.M23 * matrix2.M22 + matrix2.M23;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the division of a <see cref="Matrix2D"/> by a scalar.
    /// </summary>
    public static Matrix2D Divide(in Matrix2D matrix, float divisor) {
        Divide(matrix, divisor, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the division of a <see cref="Matrix2D"/> by a scalar and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Divide(in Matrix2D matrix, float divisor, out Matrix2D result) {
        float oneOverDiv = 1f / divisor;

        result.M11 = matrix.M11 * oneOverDiv;
        result.M12 = matrix.M12 * oneOverDiv;
        result.M13 = matrix.M13 * oneOverDiv;

        result.M21 = matrix.M21 * oneOverDiv;
        result.M22 = matrix.M22 * oneOverDiv;
        result.M23 = matrix.M23 * oneOverDiv;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the division of a <see cref="Matrix2D"/> by another.
    /// </summary>
    /// <param name="matrix1">The numerator <see cref="Matrix2D"/>.</param>
    /// <param name="matrix2">The denominator <see cref="Matrix2D"/>.</param>
    public static Matrix2D Divide(in Matrix2D matrix1, in Matrix2D matrix2) {
        Divide(matrix1, matrix2, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the division of a <see cref="Matrix2D"/> by another and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="matrix1">The numerator <see cref="Matrix2D"/>.</param>
    /// <param name="matrix2">The denominator <see cref="Matrix2D"/>.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Divide(in Matrix2D matrix1, in Matrix2D matrix2, out Matrix2D result) {
        result.M11 = matrix1.M11 / matrix2.M11;
        result.M12 = matrix1.M12 / matrix2.M12;
        result.M13 = matrix1.M13 / matrix2.M13;

        result.M21 = matrix1.M21 / matrix2.M21;
        result.M22 = matrix1.M22 / matrix2.M22;
        result.M23 = matrix1.M23 / matrix2.M23;
    }

    #endregion

    #region Helper Functions

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the inverse of this one.
    /// </summary>
    public Matrix2D Inverse() {
        Invert(this, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the inverse of the specified <see cref="Matrix2D"/> and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Invert(in Matrix2D matrix, out Matrix2D result) {
        float oneOverDet = 1f / matrix.Determinant;

        result.M11 = matrix.M22 * oneOverDet;
        result.M12 = -matrix.M12 * oneOverDet;
        result.M13 = (matrix.M23 * matrix.M21 - matrix.M13 * matrix.M22) * oneOverDet;

        result.M21 = -matrix.M21 * oneOverDet;
        result.M22 = matrix.M11 * oneOverDet;
        result.M23 = -(matrix.M23 * matrix.M11 - matrix.M13 * matrix.M12) * oneOverDet;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains the transpose of this one.
    /// </summary>
    public Matrix2D Transpose() {
        Transpose(this, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains the transpose of the specified <see cref="Matrix2D"/> and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Transpose(in Matrix2D matrix, out Matrix2D result) {
        result.M11 = matrix.M11;
        result.M12 = matrix.M21;
        result.M13 = 0f;

        result.M21 = matrix.M12;
        result.M22 = matrix.M22;
        result.M23 = 0f;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix2D"/> that contains a linear interpolation of the values in the specified matrices.
    /// </summary>
    /// <param name="value">The interpolation value.</param>
    public static Matrix2D Lerp(in Matrix2D matrix1, in Matrix2D matrix2, float value) {
        Lerp(matrix1, matrix2, value, out Matrix2D result);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="Matrix2D"/> that contains a linear interpolation of the values in the specified matrices and stores it in <paramref name="result"/>.
    /// </summary>
    /// <param name="value">The interpolation value.</param>
    /// <param name="result">The result <see cref="Matrix2D"/> as an output parameter.</param>
    public static void Lerp(in Matrix2D matrix1, in Matrix2D matrix2, float value, out Matrix2D result) {
        result.M11 = matrix2.M11 + (matrix2.M11 - matrix1.M11) * value;
        result.M12 = matrix2.M12 + (matrix2.M12 - matrix1.M12) * value;
        result.M13 = matrix2.M13 + (matrix2.M13 - matrix1.M13) * value;

        result.M21 = matrix2.M21 + (matrix2.M21 - matrix1.M21) * value;
        result.M22 = matrix2.M22 + (matrix2.M22 - matrix1.M22) * value;
        result.M23 = matrix2.M23 + (matrix2.M23 - matrix1.M23) * value;
    }

    #endregion

    #region Operators

    /// <summary>
    /// Adds two matrices.
    /// </summary>
    public static Matrix2D operator +(Matrix2D matrix1, in Matrix matrix2) {
        matrix1.M11 += matrix2.M11;
        matrix1.M12 += matrix2.M12;
        matrix1.M13 += matrix2.M13;

        matrix1.M21 += matrix2.M21;
        matrix1.M22 += matrix2.M22;
        matrix1.M23 += matrix2.M23;

        return matrix1;
    }

    /// <summary>
    /// Negates the values of a matrix.
    /// </summary>
    public static Matrix2D operator -(Matrix2D matrix) {
        matrix.M11 = -matrix.M11;
        matrix.M12 = -matrix.M12;
        matrix.M13 = -matrix.M13;

        matrix.M21 = -matrix.M21;
        matrix.M22 = -matrix.M22;
        matrix.M23 = -matrix.M23;

        return matrix;
    }

    /// <summary>
    /// Subtracts two matrices.
    /// </summary>
    public static Matrix2D operator -(Matrix2D matrix1, in Matrix matrix2) {
        matrix1.M11 -= matrix2.M11;
        matrix1.M12 -= matrix2.M12;
        matrix1.M13 -= matrix2.M13;

        matrix1.M21 -= matrix2.M21;
        matrix1.M22 -= matrix2.M22;
        matrix1.M23 -= matrix2.M23;

        return matrix1;
    }

    /// <summary>
    /// Divides a matrix's elements by the divisor.
    /// </summary>
    public static Matrix2D operator /(Matrix2D matrix, float divisor) {
        float oneOverDiv = 1f / divisor;

        matrix.M11 *= oneOverDiv;
        matrix.M12 *= oneOverDiv;
        matrix.M13 *= oneOverDiv;

        matrix.M21 *= oneOverDiv;
        matrix.M22 *= oneOverDiv;
        matrix.M23 *= oneOverDiv;

        return matrix;
    }

    /// <summary>
    /// Divides a the elements of the first matrix by the elements of the second matrix.
    /// </summary>
    public static Matrix2D operator /(Matrix2D matrix1, in Matrix2D matrix2) {
        matrix1.M11 /= matrix2.M11;
        matrix1.M12 /= matrix2.M12;
        matrix1.M13 /= matrix2.M13;

        matrix1.M21 /= matrix2.M21;
        matrix1.M22 /= matrix2.M22;
        matrix1.M23 /= matrix2.M23;

        return matrix1;
    }

    /// <summary>
    /// Multiplies a matrix by a scalar.
    /// </summary>
    public static Matrix2D operator *(Matrix2D matrix, float scalar) {
        matrix.M11 *= scalar;
        matrix.M12 *= scalar;
        matrix.M13 *= scalar;

        matrix.M21 *= scalar;
        matrix.M22 *= scalar;
        matrix.M23 *= scalar;

        return matrix;
    }

    /// <summary>
    /// Multiplies two matrices.
    /// </summary>
    public static Matrix2D operator *(Matrix2D matrix1, in Matrix2D matrix2) {
        float m11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21;
        float m12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22;
        float m13 = matrix1.M13 * matrix2.M11 + matrix1.M23 * matrix2.M21 + matrix2.M13;

        float m21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21;
        float m22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22;
        float m23 = matrix1.M13 * matrix2.M12 + matrix1.M23 * matrix2.M22 + matrix2.M23;

        matrix1.M11 = m11;
        matrix1.M12 = m12;
        matrix1.M13 = m13;

        matrix1.M21 = m21;
        matrix1.M22 = m22;
        matrix1.M23 = m23;

        return matrix1;
    }

    /// <summary>
    /// Checks if two matrices are equal with zero tolerance.
    /// </summary>
    public static bool operator ==(in Matrix2D matrix1, in Matrix2D matrix2) {
        return
            matrix1.M11 == matrix2.M11 &&
            matrix1.M12 == matrix2.M12 &&
            matrix1.M13 == matrix2.M13 &&
            matrix1.M21 == matrix2.M21 &&
            matrix1.M22 == matrix2.M22 &&
            matrix1.M23 == matrix2.M23;
    }

    /// <summary>
    /// Checks if two matrices are not equal with zero tolerance.
    /// </summary>
    public static bool operator !=(in Matrix2D matrix1, in Matrix2D matrix2) {
        return
            matrix1.M11 != matrix2.M11 ||
            matrix1.M12 != matrix2.M12 ||
            matrix1.M13 != matrix2.M13 ||
            matrix1.M21 != matrix2.M21 ||
            matrix1.M22 != matrix2.M22 ||
            matrix1.M23 != matrix2.M23;
    }

    /// <summary>
    /// Implicit cast to a <see cref="Matrix"/>.
    /// </summary>
    public static implicit operator Matrix(in Matrix2D m) {
        Matrix ret;

        ret.M11 = m.M11;
        ret.M12 = m.M12;
        ret.M13 = 0f;
        ret.M14 = 0f;
        ret.M21 = m.M21;
        ret.M22 = m.M22;
        ret.M23 = 0f;
        ret.M24 = 0f;
        ret.M31 = 0f;
        ret.M32 = 0f;
        ret.M33 = 1f;
        ret.M34 = 0f;
        ret.M41 = m.M13;
        ret.M42 = m.M23;
        ret.M43 = 0f;
        ret.M44 = 1f;

        return ret;
    }

    #endregion

    #region Object Overrides

    public bool Equals(Matrix2D other) {
        return this == other;
    }

    public override bool Equals(object obj) {
        if (obj is Matrix2D m) {
            return this == m;
        }
        return false;
    }

    public override int GetHashCode() {
        return HashCode.Combine(M11, M12, M13, M21, M22, M23);
    }

    public override string ToString() {
        return
            $"[M11 = {M11}, M12 = {M12}, M13 = {M13}]\n" +
            $"[M21 = {M21}, M22 = {M22}, M23 = {M23}]";
    }

    #endregion
}
