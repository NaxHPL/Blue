using BlueFw.Math;
using Microsoft.Xna.Framework;

namespace BlueFw;

public static class MatrixExt {

    /// <summary>
    /// Gets a <see cref="Matrix2D"/> representation for this <see cref="Matrix"/>.
    /// </summary>
    public static Matrix2D ToMatrix2D(this Matrix matrix) {
        Matrix2D matrix2D;

        matrix2D.M11 = matrix.M11;
        matrix2D.M12 = matrix.M12;

        matrix2D.M21 = matrix.M21;
        matrix2D.M22 = matrix.M22;

        matrix2D.M31 = matrix.M41;
        matrix2D.M32 = matrix.M42;

        return matrix2D;
    }
}
