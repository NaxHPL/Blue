using Microsoft.Xna.Framework;
using System;

namespace BlueFw;

/// <summary>
/// Position, scale, and rotation of an entity.
/// </summary>
public class Transform {

    [Flags]
    enum DirtyFlags {
        Clean       = 0x00,
        Position    = 0x01,
        Scale       = 0x02,
        Rotation    = 0x04,

        All = Position | Scale | Rotation
    }

    /// <summary>
    /// The parent of this <see cref="Transform"/>.
    /// </summary>
    public Transform Parent {
        get => parent;
        internal set => SetParent(value);
    }

    /// <summary>
    /// <see langword="true"/> if this <see cref="Transform"/> has a parent; otherwise <see langword="false"/>.
    /// </summary>
    public bool HasParent => parent != null;

    /// <summary>
    /// The number of children attached to this <see cref="Transform"/>.
    /// </summary>
    public int ChildCount => children.Length;

    /// <summary>
    /// The topmost <see cref="Transform"/> in the hierarchy.
    /// </summary>
    public Transform Root => HasParent ? parent.Root : this;

    /// <summary>
    /// The world space position of this <see cref="Transform"/>.
    /// </summary>
    public Vector2 Position {
        get { UpdateTransform(); UpdatePosition(); return position; }
        set => SetPosition(value);
    }

    /// <summary>
    /// The world space scale of this <see cref="Transform"/>.
    /// </summary>
    public Vector2 Scale {
        get { UpdateTransform(); return scale; }
        set => SetScale(value);
    }

    /// <summary>
    /// The world space rotation of this <see cref="Transform"/> in radians.
    /// </summary>
    public float Rotation {
        get { UpdateTransform(); return rotation; }
        set => SetRotation(value);
    }

    /// <summary>
    /// The world space rotation of this <see cref="Transform"/> in degrees.
    /// </summary>
    public float RotationDegrees {
        get => MathHelper.ToDegrees(Rotation);
        set => SetRotation(MathHelper.ToRadians(value));
    }

    /// <summary>
    /// The position of this <see cref="Transform"/> relative to its parent.
    /// </summary>
    public Vector2 LocalPosition {
        get { UpdateTransform(); return localPosition; }
        set => SetLocalPosition(value);
    }

    /// <summary>
    /// The scale of this <see cref="Transform"/> relative to its parent.
    /// </summary>
    public Vector2 LocalScale {
        get { UpdateTransform(); return localScale; }
        set => SetLocalScale(value);
    }

    /// <summary>
    /// The rotation of this <see cref="Transform"/> relative to its parent in radians.
    /// </summary>
    public float LocalRotation {
        get { UpdateTransform(); return localRotation; }
        set => SetLocalRotation(value);
    }

    /// <summary>
    /// The rotation of this <see cref="Transform"/> relative to its parent in degrees.
    /// </summary>
    public float LocalRotationDegrees {
        get => MathHelper.ToDegrees(LocalRotation);
        set => SetLocalRotation(MathHelper.ToRadians(value));
    }

    /// <summary>
    /// A <see cref="Matrix2D"/> that transforms a point from local space to world space.
    /// </summary>
    public Matrix2D LocalToWorldMatrix {
        get { UpdateTransform(); return worldMatrix; }
    }

    /// <summary>
    /// A <see cref="Matrix2D"/> that transforms a point from world space to local space.
    /// </summary>
    public Matrix2D WorldToLocalMatrix {
        get { UpdateWorldToLocal(); return worldToLocalMatrix; }
    }

    /// <summary>
    /// The positive X direction of this <see cref="Transform"/> in local space.
    /// </summary>
    public Vector2 Right {
        get { UpdateWorldToLocal(); return right; }
    }

    /// <summary>
    /// The positive Y direction of this <see cref="Transform"/> in local space.
    /// </summary>
    public Vector2 Down {
        get { UpdateWorldToLocal(); return down; }
    }

    /// <summary>
    /// The entity this <see cref="Transform"/> is attached to.
    /// </summary>
    public readonly Entity Entity;

    Transform parent;
    readonly FastList<Transform> children = new FastList<Transform>();

    Vector2 position = Vector2.Zero;
    Vector2 scale = Vector2.One;
    float rotation = 0f;

    Vector2 localPosition = Vector2.Zero;
    Vector2 localScale = Vector2.One;
    float localRotation = 0f;

    Matrix2D translationMatrix;
    Matrix2D scaleMatrix;
    Matrix2D rotationMatrix;

    Matrix2D localMatrix = Matrix2D.Identity;
    Matrix2D worldMatrix = Matrix2D.Identity;
    Matrix2D worldToLocalMatrix = Matrix2D.Identity;

    Vector2 right = Vector2.UnitX;
    Vector2 down = Vector2.UnitY;

    DirtyFlags hierarchyDirty = DirtyFlags.All;
    DirtyFlags localDirty = DirtyFlags.All;
    bool positionDirty = true;
    bool worldToLocalDirty = true;

    /// <summary>
    /// Creates a new <see cref="Transform"/>.
    /// </summary>
    /// <param name="entity">The entity this transform is attached to.</param>
    public Transform(Entity entity) {
        Entity = entity;
    }

    #region Set World

    /// <summary>
    /// Sets the world space position of this transform.
    /// </summary>
    public void SetPosition(float posX, float posY) {
        SetPosition(new Vector2(posX, posY));
    }

    /// <summary>
    /// Sets the world space position of this transform.
    /// </summary>
    public void SetPosition(in Vector2 position) {
        if (this.position == position) {
            return;
        }

        this.position = position;
        LocalPosition = HasParent ? Vector2Ext.Transform(position, WorldToLocalMatrix) : position;

        positionDirty = false;
    }

    /// <summary>
    /// Sets the world space scale of this transform. Both the X and Y scale values are set to <paramref name="scale"/>.
    /// </summary>
    public void SetScale(float scale) {
        SetScale(new Vector2(scale, scale));
    }

    /// <summary>
    /// Sets the world space scale of this transform.
    /// </summary>
    public void SetScale(float scaleX, float scaleY) {
        SetScale(new Vector2(scaleX, scaleY));
    }

    /// <summary>
    /// Sets the world space scale of this transform.
    /// </summary>
    public void SetScale(in Vector2 scale) {
        if (this.scale == scale) {
            return;
        }

        this.scale = scale;
        LocalScale = HasParent ? scale / parent.Scale : scale;
    }

    /// <summary>
    /// Sets the world space rotation (degrees) of this transform.
    /// </summary>
    public void SetRotationDegrees(float degrees) {
        SetRotation(MathHelper.ToRadians(degrees));
    }

    /// <summary>
    /// Sets the world space rotation (radians) of this transform.
    /// </summary>
    public void SetRotation(float radians) {
        if (rotation == radians) {
            return;
        }

        rotation = radians;
        LocalRotation = HasParent ? parent.Rotation + radians : radians;
    }

    #endregion

    #region Set Local

    /// <summary>
    /// Sets the local space position of this transform.
    /// </summary>
    public void SetLocalPosition(float posX, float posY) {
        SetLocalPosition(new Vector2(posX, posY));
    }

    /// <summary>
    /// Sets the local space position of this transform.
    /// </summary>
    public void SetLocalPosition(in Vector2 position) {
        if (localPosition == position) {
            return;
        }

        localPosition = position;

        localDirty = DirtyFlags.All;
        positionDirty = true;
        SetHierarchyDirty(DirtyFlags.Position);
    }

    /// <summary>
    /// Sets the local space scale of this transform. Both the X and Y scale values are set to <paramref name="scale"/>.
    /// </summary>
    public void SetLocalScale(float scale) {
        SetLocalScale(new Vector2(scale, scale));
    }

    /// <summary>
    /// Sets the local space scale of this transform.
    /// </summary>
    public void SetLocalScale(float scaleX, float scaleY) {
        SetLocalScale(new Vector2(scaleX, scaleY));
    }

    /// <summary>
    /// Sets the local space scale of this transform.
    /// </summary>
    public void SetLocalScale(in Vector2 scale) {
        if (localScale == scale) {
            return;
        }

        localScale = scale;

        localDirty |= DirtyFlags.Scale;
        positionDirty = true;
        SetHierarchyDirty(DirtyFlags.Scale);
    }

    /// <summary>
    /// Sets the local space rotation (degrees) of this transform.
    /// </summary>
    public void SetLocalRotationDegrees(float degrees) {
        SetLocalRotation(MathHelper.ToRadians(degrees));
    }

    /// <summary>
    /// Sets the local space rotation (radians) of this transform.
    /// </summary>
    public void SetLocalRotation(float radians) {
        if (localRotation == radians) {
            return;
        }

        localRotation = radians;

        localDirty = DirtyFlags.All;
        positionDirty = true;
        SetHierarchyDirty(DirtyFlags.Rotation);
    }

    #endregion

    void SetHierarchyDirty(DirtyFlags dirtyFlags) {
        if (hierarchyDirty.HasFlag(dirtyFlags)) {
            return;
        }

        hierarchyDirty |= dirtyFlags;

        for (int i = 0; i < Entity.Components.Count; i++) {
            Entity.Components[i].InvokeOnEntityTransformChanged();
        }

        for (int i = 0; i < children.Length; i++) {
            children.Buffer[i].SetHierarchyDirty(dirtyFlags);
        }
    }

    void UpdateTransform() {
        if (hierarchyDirty == DirtyFlags.Clean) {
            return;
        }

        if (HasParent) {
            parent.UpdateTransform();
        }

        if (localDirty != DirtyFlags.Clean) {
            if (localDirty.HasFlag(DirtyFlags.Position)) {
                Matrix2D.CreateTranslation(localPosition, out translationMatrix);
            }

            if (localDirty.HasFlag(DirtyFlags.Rotation)) {
                Matrix2D.CreateRotation(localRotation, out rotationMatrix);
            }

            if (localDirty.HasFlag(DirtyFlags.Scale)) {
                Matrix2D.CreateScale(localScale, out scaleMatrix);
            }

            Matrix2D.Multiply(scaleMatrix, rotationMatrix, out localMatrix);
            Matrix2D.Multiply(localMatrix, translationMatrix, out localMatrix);

            if (!HasParent) {
                scale = localScale;
                rotation = localRotation;
                worldMatrix = localMatrix;
            }

            localDirty = DirtyFlags.Clean;
        }

        if (HasParent) {
            Matrix2D.Multiply(localMatrix, parent.worldMatrix, out worldMatrix);
            rotation = parent.rotation + localRotation;
            scale = parent.scale * localScale;
        }

        worldToLocalDirty = true;
        positionDirty = true;
        hierarchyDirty = DirtyFlags.Clean;
    }

    void UpdatePosition() {
        if (!positionDirty) {
            return;
        }

        if (HasParent) {
            parent.UpdateTransform();
            Vector2Ext.Transform(localPosition, parent.worldMatrix, out position);
        }
        else {
            position = localPosition;
        }

        positionDirty = false;
    }

    void UpdateWorldToLocal() {
        if (!worldToLocalDirty) {
            return;
        }

        if (HasParent) {
            parent.UpdateTransform();
            Matrix2D.Invert(parent.worldMatrix, out worldToLocalMatrix);
        }
        else {
            worldToLocalMatrix = Matrix2D.Identity;
        }

        Vector2Ext.Transform(Vector2.UnitX, worldToLocalMatrix, out right);
        Vector2Ext.Transform(Vector2.UnitY, worldToLocalMatrix, out down);

        worldToLocalDirty = false;
    }

    /// <summary>
    /// Transforms <paramref name="position"/> from local space to world space.
    /// </summary>
    public Vector2 TransformPoint(in Vector2 position) {
        TransformPoint(position, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Transforms <paramref name="position"/> from local space to world space and stores it in <paramref name="result"/>.
    /// </summary>
    public void TransformPoint(in Vector2 position, out Vector2 result) {
        Vector2Ext.Transform(position, LocalToWorldMatrix, out result);
    }

    /// <summary>
    /// Transforms <paramref name="position"/> from world space to local space.
    /// </summary>
    public Vector2 InverseTransformPoint(in Vector2 position) {
        InverseTransformPoint(position, out Vector2 result);
        return result;
    }

    /// <summary>
    /// Transforms <paramref name="position"/> from world space to local space and stores it in <paramref name="result"/>.
    /// </summary>
    public void InverseTransformPoint(in Vector2 position, out Vector2 result) {
        Vector2Ext.Transform(position, WorldToLocalMatrix, out result);
    }

    /// <summary>
    /// Rotates this <see cref="Transform"/> to look at <paramref name="target"/>'s world space position.
    /// </summary>
    public void LookAt(Transform target) {
        LookAt(target.Position);
    }

    /// <summary>
    /// Rotates this <see cref="Transform"/> to look at <paramref name="position"/>.
    /// </summary>
    /// <param name="position">A world space position.</param>
    public void LookAt(in Vector2 position) {
        Vector2 vectorToAlignTo = Position - position;
        vectorToAlignTo.Normalize();

        int sign = Position.X > position.X ? -1 : 1;
        float dot = Vector2.Dot(vectorToAlignTo, Vector2.UnitY);

        SetRotation(sign * MathF.Acos(dot));
    }

    /// <summary>
    /// Applies a rotation of <paramref name="angle"/> degrees.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <param name="relativeTo">(Optional) Determines whether to rotate relative to itself or to the world.</param>
    public void RotateDegrees(float angle, Space relativeTo = Space.Local) {
        Rotate(MathHelper.ToRadians(angle), relativeTo);
    }

    /// <summary>
    /// Applies a rotation of <paramref name="angle"/> radians.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <param name="relativeTo">(Optional) Determines whether to rotate relative to itself or to the world.</param>
    public void Rotate(float angle, Space relativeTo = Space.Local) {
        if (relativeTo == Space.Local) {
            LocalRotation += angle;
        }
        else {
            Rotation += angle;
        }
    }

    /// <summary>
    /// Moves this transform <paramref name="x"/> units along the X axis and <paramref name="y"/> units along the Y axis.
    /// </summary>
    /// <param name="relativeTo">(Optional) Determines whether to move relative to itself or to the world.</param>
    public void Translate(float x, float y, Space relativeTo = Space.Local) {
        Translate(new Vector2(x, y), relativeTo);
    }

    /// <summary>
    /// Moves this transform in the direction and distance of <paramref name="translation"/>.
    /// </summary>
    /// <param name="relativeTo">(Optional) Determines whether to move relative to itself or to the world.</param>
    public void Translate(in Vector2 translation, Space relativeTo = Space.Local) {
        if (relativeTo == Space.Local) {
            LocalPosition += translation;
        }
        else {
            Position += translation;
        }
    }

    /// <summary>
    /// Sets the parent of this <see cref="Transform"/>.
    /// </summary>
    /// <param name="transform">The new parent transform.</param>
    internal void SetParent(Transform transform) {
        if (parent == transform) {
            return;
        }

        if (HasParent) {
            parent.children.Remove(this);
        }

        if (transform != null) {
            transform.children.Add(this);
        }

        parent = transform;

        worldToLocalDirty = true;
        SetHierarchyDirty(DirtyFlags.All);
    }

    /// <summary>
    /// Removes this <see cref="Transform"/> from its parent.
    /// </summary>
    internal void DetachFromParent() {
        SetParent(null);
    }

    /// <summary>
    /// Detaches all children transforms from this one.
    /// </summary>
    internal void DetachChildren() {
        for (int i = children.Length - 1; i >= 0; i--) {
            children.Buffer[i].DetachFromParent();
        }
    }

    /// <summary>
    /// Returns the child <see cref="Transform"/> at the specified index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Transform GetChildAt(int index) {
        if (index < 0 || index >= children.Length) {
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Expected a value in the range [0,{children.Length - 1}]");
        }

        return children.Buffer[index];
    }

    /// <summary>
    /// Returns the index of the specified child <see cref="Transform"/>, or -1 if the child was not found.
    /// </summary>
    public int GetIndexOf(Transform child) {
        return children.IndexOf(child);
    }

    /// <summary>
    /// Returns a boolean value that indicates whether this <see cref="Transform"/> is a child of <paramref name="transform"/>.
    /// </summary>
    /// <param name="deepSearch">If <see langword="true"/> (default), this method will also check if this <see cref="Transform"/> is a child of a child of <paramref name="transform"/>.</param>
    public bool IsChildOf(Transform transform, bool deepSearch = true) {
        if (transform == parent) {
            return true;
        }

        if (deepSearch && HasParent) {
            return parent.IsChildOf(transform, deepSearch);
        }

        return false;
    }
}
