using Microsoft.Xna.Framework;
using System;

namespace Blue.Math;

/// <summary>
/// Describes a 2D rectangle. Does <b>not</b> support negative width/height.
/// </summary>
public struct RectangleF {

    #region Statics

    /// <summary>
    /// A <see cref="RectangleF"/> with X = 0, Y = 0, Width = 0, and Height = 0.
    /// </summary>
    public static RectangleF Zero => zero;
    static RectangleF zero = new RectangleF();

    /// <summary>
    /// Returns <see langword="true"/> if the provided rectangles overlap each other.
    /// </summary>
    public static bool Overlaps(in RectangleF rect1, in RectangleF rect2) {
        Overlaps(rect1, rect2, out bool result);
        return result;
    }

    /// <summary>
    /// Checks if the provided rectangles overlap each other and stores the result in <paramref name="result"/>.
    /// </summary>
    public static void Overlaps(in RectangleF rect1, in RectangleF rect2, out bool result) {
        result =
            rect1.MinX < rect2.MaxX &&
            rect1.MaxX > rect2.MinX &&
            rect1.MinY < rect2.MaxY &&
            rect1.MaxY > rect2.MinY;
    }

    /// <summary>
    /// Creates a new <see cref="RectangleF"/> that contains the overlapping region of the provided rectangles.
    /// </summary>
    public static RectangleF Intersect(in RectangleF rect1, in RectangleF rect2) {
        Intersect(rect1, rect2, out RectangleF result);
        return result;
    }

    /// <summary>
    /// Gets the overlapping region of the provided rectangles and stores it in <paramref name="result"/>.
    /// </summary>
    public static void Intersect(in RectangleF rect1, in RectangleF rect2, out RectangleF result) {
        if (!Overlaps(rect1, rect2)) {
            result = zero;
            return;
        }

        float left = MathHelper.Max(rect1.MinX, rect2.MinX);
        float right = MathHelper.Min(rect1.MaxX, rect2.MaxX);
        float top = MathHelper.Max(rect1.MinY, rect2.MinY);
        float bottom = MathHelper.Min(rect1.MaxY, rect2.MaxY);

        result.X = left;
        result.Y = top;
        result.Width = right - left;
        result.Height = bottom - top;
    }

    /// <summary>
    /// Creates a new <see cref="RectangleF"/> that completely contains the provided rectangles.
    /// </summary>
    public static RectangleF Union(in RectangleF rect1, in RectangleF rect2) {
        Union(rect1, rect2, out RectangleF result);
        return result;
    }

    /// <summary>
    /// Gets a <see cref="RectangleF"/> that completely contains the provided rectangles and stores it in <paramref name="result"/>.
    /// </summary>
    public static void Union(in RectangleF rect1, in RectangleF rect2, out RectangleF result) {
        float left = MathHelper.Min(rect1.MinX, rect2.MinX);
        float right = MathHelper.Max(rect1.MaxX, rect2.MaxX);
        float top = MathHelper.Min(rect1.MinY, rect2.MinY);
        float bottom = MathHelper.Max(rect1.MaxY, rect2.MaxY);

        result.X = left;
        result.Y = top;
        result.Width = right - left;
        result.Height = bottom - top;
    }

    /// <summary>
    /// Returns a point inside a <see cref="RectangleF"/> given normalized coordinates.
    /// </summary>
    public static Vector2 NormalizedToPoint(in RectangleF rect, in Vector2 normalizedCoords) {
        NormalizedToPoint(rect, normalizedCoords, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Gets a point inside a <see cref="RectangleF"/> given normalized coordinates and stores it in <paramref name="result"/>.
    /// </summary>
    public static void NormalizedToPoint(in RectangleF rect, in Vector2 normalizedCoords, out Vector2 result) {
        result.X = rect.X + rect.Width * normalizedCoords.X;
        result.Y = rect.Y + rect.Height * normalizedCoords.Y;
    }

    /// <summary>
    /// Returns the normalized coordinates corresponding the <paramref name="point"/>.
    /// </summary>
    public static Vector2 PointToNormalized(in RectangleF rect, in Vector2 point) {
        PointToNormalized(rect, point, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Gets the normalized coordinates corresponding the <paramref name="point"/> and stores them in <paramref name="result"/>.
    /// </summary>
    public static void PointToNormalized(in RectangleF rect, in Vector2 point, out Vector2 result) {
        result.X = (point.X - rect.X) / rect.Width;
        result.Y = (point.Y - rect.Y) / rect.Height;
    }

    /// <summary>
    /// Returns the normalized coordinates corresponding the <paramref name="point"/>. The coordinates are clamped between 0 and 1.
    /// </summary>
    public static Vector2 PointToNormalizedClamped(in RectangleF rect, in Vector2 point) {
        PointToNormalizedClamped(rect, point, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Gets the normalized coordinates corresponding the <paramref name="point"/> and stores them in <paramref name="result"/>. The coordinates are clamped between 0 and 1.
    /// </summary>
    public static void PointToNormalizedClamped(in RectangleF rect, in Vector2 point, out Vector2 result) {
        result.X = MathHelper.Clamp((point.X - rect.X) / rect.Width, 0f, 1f);
        result.Y = MathHelper.Clamp((point.Y - rect.Y) / rect.Height, 0f, 1f);
    }

    /// <summary>
    /// Creates a new <see cref="RectangleF"/> from min/max coordinate values.
    /// </summary>
    public static RectangleF FromMinMax(float minX, float maxX, float minY, float maxY) {
        return new RectangleF(minX, minY, maxX - minX, maxY - minY);
    }

    /// <summary>
    /// Creates a new <see cref="RectangleF"/> from min/max coordinate values.
    /// </summary>
    public static RectangleF FromMinMax(in Vector2 min, in Vector2 max) {
        return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// A point located at the center of the rectangle.
    /// </summary>
    public Vector2 Center => new Vector2(X + Width * 0.5f, Y + Height * 0.5f);

    /// <summary>
    /// The X and Y position of the rectangle.
    /// </summary>
    public Vector2 Position {
        get => new Vector2(X, Y);
        set {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>
    /// The width and height of the rectangle.
    /// </summary>
    public Vector2 Size {
        get => new Vector2(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    /// <summary>
    /// The minimum X coordinate of the rectangle.
    /// </summary>
    /// <remarks>Setting this value will change the X coordinate and the width to preserve <see cref="MaxX"/>.</remarks>
    public float MinX {
        get => X;
        set {
            Width -= value - X;
            X = value;
        }
    }

    /// <summary>
    /// The maximum X coordinate of the rectangle.
    /// </summary>
    /// <remarks>Setting this value will change the width to preserve <see cref="MinX"/>.</remarks>
    public float MaxX {
        get => X + Width;
        set => Width += value - X + Width;
    }

    /// <summary>
    /// The minimum Y coordinate of the rectangle.
    /// </summary>
    /// <remarks>Setting this value will change the Y coordinate and the height to preserve <see cref="MaxY"/>.</remarks>
    public float MinY {
        get => Y;
        set {
            Height -= value - Y;
            Y = value;
        }
    }

    /// <summary>
    /// The maximum Y coordinate of the rectangle.
    /// </summary>
    /// <remarks>Setting this value will change the height to preserve <see cref="MinY"/>.</remarks>
    public float MaxY {
        get => Y + Height;
        set => Height += value - X + Height;
    }

    /// <summary>
    /// The position of the minimum (top left) corner of the rectangle.
    /// </summary>
    /// <remarks>Setting this value will change the rectangle's position and size to preserve <see cref="Max"/>.</remarks>
    public Vector2 Min {
        get => new Vector2(X, Y);
        set {
            MinX = value.X;
            MinY = value.Y;
        }
    }

    /// <summary>
    /// The position of the maximum (bottom right) corner of the rectangle.
    /// </summary>
    /// <remarks>Setting this value will change the width and height to preserve <see cref="Min"/>.</remarks>
    public Vector2 Max {
        get => new Vector2(X + Width, Y + Width);
        set {
            MaxX = value.X;
            MaxY = value.Y;
        }
    }

    #endregion

    #region Public Fields

    /// <summary>
    /// The X coordinate of the rectangle.
    /// </summary>
    public float X;

    /// <summary>
    /// The Y coordinate of the rectangle.
    /// </summary>
    public float Y;

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public float Width;

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public float Height;

    #endregion

    /// <summary>
    /// Creates a new <see cref="RectangleF"/> with the provided position and size.
    /// </summary>
    /// <param name="position">The rectangle's position.</param>
    /// <param name="size">The rectangle's size.</param>
    public RectangleF(in Vector2 position, in Vector2 size) {
        X = position.X;
        Y = position.Y;
        Width = size.X;
        Height = size.Y;
    }

    /// <summary>
    /// Creates a new <see cref="RectangleF"/> with the provided x, y, width, and height.
    /// </summary>
    /// <param name="x">The X coordinate of the rectangle.</param>
    /// <param name="y">The Y coordinate of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    public RectangleF(float x, float y, float width, float height) {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="point"/> lies within the rectangle.
    /// </summary>
    public bool Contains(in Point point) {
        return
            point.X > X &&
            point.X < X + Width &&
            point.Y > Y &&
            point.Y < Y + Height;
    }

    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="point"/> lies within the rectangle.
    /// </summary>
    public bool Contains(in Vector2 point) {
        return
            point.X > X &&
            point.X < X + Width &&
            point.Y > Y &&
            point.Y < Y + Height;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the provided X and Y coordinates lie within the rectangle.
    /// </summary>
    public bool Contains(float x, float y) {
        return
            x > X &&
            x < X + Width &&
            y > Y &&
            y < Y + Height;
    }

    /// <summary>
    /// Checks if <paramref name="point"/> lies within the rectangle and stores the result in <paramref name="result"/>.
    /// </summary>
    public void Contains(in Point point, out bool result) {
        result =
            point.X > X &&
            point.X < X + Width &&
            point.Y > Y &&
            point.Y < Y + Height;
    }

    /// <summary>
    /// Checks if <paramref name="point"/> lies within the rectangle and stores the result in <paramref name="result"/>.
    /// </summary>
    public void Contains(in Vector2 point, out bool result) {
        result =
            point.X > X &&
            point.X < X + Width &&
            point.Y > Y &&
            point.Y < Y + Height;
    }

    /// <summary>
    /// Checks if the provided X and Y coordinates lie within the rectangle and stores the result in <paramref name="result"/>.
    /// </summary>
    public void Contains(float x, float y, out bool result) {
        result =
            x > X &&
            x < X + Width &&
            y > Y &&
            y < Y + Height;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the provided rectangle is completely contained within this one.
    /// </summary>
    public bool Contains(in RectangleF other) {
        return
            other.MinX > MinX &&
            other.MaxX < MaxX &&
            other.MinY > MinY &&
            other.MaxY < MaxY;
    }

    /// <summary>
    /// Checks if the provided rectangle is completely contained within this one and stores the result in <paramref name="result"/>.
    /// </summary>
    public void Contains(in RectangleF other, out bool result) {
        result =
            other.MinX > MinX &&
            other.MaxX < MaxX &&
            other.MinY > MinY &&
            other.MaxY < MaxY;
    }

    /// <summary>
    /// Returns <see langword="true"/> if the provided rectangle overlaps this one.
    /// </summary>
    public bool Overlaps(in RectangleF other) {
        return
            other.MinX < MaxX &&
            other.MaxX > MinX &&
            other.MinY < MaxY &&
            other.MaxY > MinY;
    }

    /// <summary>
    /// Checks if the provided rectangle overlaps this one and stores the result in <paramref name="result"/>.
    /// </summary>
    public void Overlaps(in RectangleF other, out bool result) {
        result =
            other.MinX < MaxX &&
            other.MaxX > MinX &&
            other.MinY < MaxY &&
            other.MaxY > MinY;
    }

    /// <summary>
    /// Deconstructs the rectangle.
    /// </summary>
    public void Deconstruct(out float x, out float y, out float width, out float height) {
        x = X;
        y = Y;
        width = Width;
        height = Height;
    }

    /// <summary>
    /// Adjusts the edges of the rectangle by the specified amount.
    /// </summary>
    public void Inflate(float amount) {
        X -= amount;
        Y -= amount;
        Width += amount * 2f;
        Height += amount * 2f;
    }

    /// <summary>
    /// Adjusts the edges of the rectangle by the specified horizontal and vertical amounts.
    /// </summary>
    public void Inflate(float horizontalAmount, float verticalAmount) {
        X -= horizontalAmount;
        Y -= verticalAmount;
        Width += horizontalAmount * 2f;
        Height += verticalAmount * 2f;
    }

    /// <summary>
    /// Changes the position of the rectangle.
    /// </summary>
    /// <param name="offset"></param>
    public void Offset(in Vector2 offset) {
        X += offset.X;
        Y += offset.Y;
    }

    /// <summary>
    /// Changes the position of the rectangle.
    /// </summary>
    public void Offset(float offsetX, float offsetY) {
        X += offsetX;
        Y += offsetY;
    }

    #region Operators

    public static bool operator ==(in RectangleF rect1, in RectangleF rect2) {
        return
            rect1.X == rect2.X &&
            rect1.Y == rect2.Y &&
            rect1.Width == rect2.Width &&
            rect1.Height == rect2.Height;
    }

    public static bool operator !=(in RectangleF rect1, in RectangleF rect2) {
        return
            rect1.X != rect2.X ||
            rect1.Y != rect2.Y ||
            rect1.Width != rect2.Width ||
            rect1.Height != rect2.Height;
    }

    public static implicit operator Rectangle(in RectangleF rect) {
        return new Rectangle(
            (int)MathF.Round(rect.X),
            (int)MathF.Round(rect.Y),
            (int)MathF.Round(rect.Width),
            (int)MathF.Round(rect.Height)
        );
    }

    #endregion

    #region Object Overrides

    /// <summary>
    /// Returns a string representation of this rectangle.
    /// </summary>
    public override string ToString() {
        return $"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }

    public override bool Equals(object obj) {
        return obj is RectangleF r && this == r;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y, Width, Height);
    }

    #endregion
}
