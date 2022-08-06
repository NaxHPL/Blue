using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

/// <summary>
/// An orthographic camera component.
/// </summary>
public class Camera : Component {

    /// <summary>
    /// The camera's origin. Default is the center of the screen.
    /// </summary>
    public Vector2 Origin {
        get => origin;
        set => SetOrigin(value);
    }

    /// <summary>
    /// The camera's origin normalized. Default is the center of the screen.
    /// </summary>
    public Vector2 OriginNormalized {
        get => new Vector2(origin.X / Screen.Width, origin.Y / Screen.Height);
        set => SetOrigin(new Vector2(Screen.Width * value.X, Screen.Height * value.Y));
    }

    /// <summary>
    /// The camera's zoom level. Default = 1.
    /// </summary>
    public float Zoom {
        get => zoom;
        set => SetZoom(value);
    }

    /// <summary>
    /// The camera's transform matrix.
    /// </summary>
    public Matrix2D TransformMatrix {
        get { UpdateMatrices(); return transformMatrix; }
    }

    /// <summary>
    /// The inverse of the camera's transform matrix.
    /// </summary>
    public Matrix2D InverseTransformMatrix {
        get { UpdateMatrices(); return inverseTransformMatrix; }
    }

    /// <summary>
    /// The cmaera's projection matrix.
    /// </summary>
    public Matrix Projection {
        get {
            if (projectionDirty) {
                Viewport viewport = Blue.Instance.GraphicsDevice.Viewport;
                Matrix.CreateOrthographicOffCenter(0f, viewport.Width, viewport.Height, 0f, 0f, -1f, out projectionMatrix);
                projectionDirty = false;
            }

            return projectionMatrix;
        }
    }

    /// <summary>
    /// The world space bounds of this camera.
    /// </summary>
    public RectangleF Bounds {
        get { UpdateBounds(); return bounds; }
    }

    /// <summary>
    /// The camera's position.
    /// </summary>
    public Vector2 Position {
        get => Entity.Transform.Position;
        set => Entity.Transform.SetPosition(value);
    }

    /// <summary>
    /// The camera's rotation in radians.
    /// </summary>
    public float Rotation {
        get => Entity.Transform.Rotation;
        set => Entity.Transform.SetRotation(value);
    }

    /// <summary>
    /// The camera's rotation in degrees.
    /// </summary>
    public float RotationDegrees {
        get => Entity.Transform.RotationDegrees;
        set => Entity.Transform.SetRotationDegrees(value);
    }

    /// <summary>
    /// The destination texture this camera will render to. If <see langword="null"/> (default), the camera will render directly to the back buffer.
    /// </summary>
    public RenderTarget2D RenderTarget;

    /// <summary>
    /// The color this camera uses to clear the background.
    /// </summary>
    /// <remarks>By default, this is set to <see cref="Color.Black"/>.</remarks>
    public Color ClearColor = Color.Black;

    Vector2 origin;
    float zoom = 1f;
    RectangleF bounds;

    Matrix2D transformMatrix;
    Matrix2D inverseTransformMatrix;
    Matrix projectionMatrix;
    Matrix2D tempMatrix;

    bool matricesDirty = true;
    bool projectionDirty = true;
    bool boundsDirty = true;

    /// <summary>
    /// Creates a <see cref="Camera"/>.
    /// </summary>
    public Camera() {
        OriginNormalized = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// Sets the camera's origin.
    /// </summary>
    public void SetOrigin(float x, float y) {
        SetOrigin(new Vector2(x, y));
    }

    /// <summary>
    /// Sets the camera's origin.
    /// </summary>
    public void SetOrigin(in Vector2 origin) {
        if (this.origin == origin) {
            return;
        }

        this.origin = origin;
        matricesDirty = true;
    }

    /// <summary>
    /// Sets the camera's zoom level.
    /// </summary>
    public void SetZoom(float zoom) {
        if (this.zoom == zoom) {
            return;
        }

        this.zoom = zoom;
        matricesDirty = true;
    }

    public override void OnEntityTransformChanged() {
        matricesDirty = true;
    }

    protected override void OnDestroy() {
        RenderTarget?.Dispose();
    }

    /// <summary>
    /// Transforms a point from screen space to world space.
    /// </summary>
    public Vector2 ScreenToWorldPoint(in Vector2 point) {
        ScreenToWorldPoint(point, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Transforms a point from screen space to world space and stores it in <paramref name="result"/>.
    /// </summary>
    public void ScreenToWorldPoint(in Vector2 point, out Vector2 result) {
        UpdateMatrices();
        Vector2Ext.Transform(point, inverseTransformMatrix, out result);
    }

    /// <summary>
    /// Transforms a point from world space to screen space.
    /// </summary>
    public Vector2 WorldToScreenPoint(in Vector2 point) {
        WorldToScreenPoint(point, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Transforms a point from world space to screen space and stores it in <paramref name="result"/>.
    /// </summary>
    public void WorldToScreenPoint(in Vector2 point, out Vector2 result) {
        UpdateMatrices();
        Vector2Ext.Transform(point, transformMatrix, out result);
    }

    void UpdateMatrices() {
        if (!matricesDirty) {
            return;
        }

        Matrix2D.CreateTranslation(-Entity.Transform.Position, out transformMatrix);

        if (zoom != 1f) {
            Matrix2D.CreateScale(zoom, out tempMatrix);
            Matrix2D.Multiply(transformMatrix, tempMatrix, out transformMatrix);
        }

        if (Entity.Transform.Rotation != 0f) {
            Matrix2D.CreateRotation(Entity.Transform.Rotation, out tempMatrix);
            Matrix2D.Multiply(transformMatrix, tempMatrix, out transformMatrix);
        }

        Matrix2D.CreateTranslation(origin, out tempMatrix);
        Matrix2D.Multiply(transformMatrix, tempMatrix, out transformMatrix);

        Matrix2D.Invert(transformMatrix, out inverseTransformMatrix);

        boundsDirty = true;
        matricesDirty = false;
    }

    void UpdateBounds() {
        if (!boundsDirty) {
            return;
        }

        Viewport viewport = Blue.Instance.GraphicsDevice.Viewport;

        Vector2 topLeft = ScreenToWorldPoint(new Vector2(viewport.X, viewport.Y));
        Vector2 bottomRight = ScreenToWorldPoint(new Vector2(viewport.Width, viewport.Height));

        if (Entity.Transform.Rotation == 0f) {
            bounds.Position = topLeft;
            bounds.Width = bottomRight.X - topLeft.X;
            bounds.Height = bottomRight.Y - topLeft.Y;
        }
        else {
            Vector2 topRight = ScreenToWorldPoint(new Vector2(viewport.X + viewport.Width, viewport.Y));
            Vector2 bottomLeft = ScreenToWorldPoint(new Vector2(viewport.X, viewport.Y + viewport.Height));

            float minX = MathHelper.Min(topLeft.X, MathHelper.Min(topRight.X, MathHelper.Min(bottomLeft.X, bottomRight.X)));
            float maxX = MathHelper.Max(topLeft.X, MathHelper.Max(topRight.X, MathHelper.Max(bottomLeft.X, bottomRight.X)));
            float minY = MathHelper.Min(topLeft.Y, MathHelper.Min(topRight.Y, MathHelper.Min(bottomLeft.Y, bottomRight.Y)));
            float maxY = MathHelper.Max(topLeft.Y, MathHelper.Max(topRight.Y, MathHelper.Max(bottomLeft.Y, bottomRight.Y)));

            bounds.X = minX;
            bounds.Y = minY;
            bounds.Width = maxX - minX;
            bounds.Height = maxY - minY;
        }

        boundsDirty = false;
    }
}
