using System.Collections.Generic;

namespace BlueFw;

/// <summary>
/// Base class for all Blue objects.
/// </summary>
public abstract class BlueObject {

    #region Statics

    static uint nextInstanceId = 0;

    static readonly FastList<BlueObject> objectsToDestroy = new FastList<BlueObject>();
    static readonly HashSet<uint> objectsToDestroyInstanceIds = new HashSet<uint>();

    /// <summary>
    /// Destroys an entity or component.
    /// Actual object destruction is delayed until after the current Update loop, but before rendering.
    /// </summary>
    public static void Destroy(BlueObject obj) {
        if (obj == null || obj.isDestroyed) {
            return;
        }

        if (objectsToDestroyInstanceIds.Add(obj.InstanceID)) {
            objectsToDestroy.Add(obj);
        }
    }

    /// <summary>
    /// Destroys an entity or component immediately.
    /// It is recommended to use <see cref="Destroy(BlueObject)"/> instead.
    /// </summary>
    public static void DestroyImmediate(BlueObject obj) {
        if (obj == null || obj.isDestroyed) {
            return;
        }

        obj.Destroy();
        obj.isDestroyed = true;
    }

    /// <summary>
    /// Immediately destroy objects that have been queued up for destruction.
    /// </summary>
    internal static void DestroyQueuedObjects() {
        if (objectsToDestroy.Length > 0) {
            for (int i = 0; i < objectsToDestroy.Length; i++) {
                DestroyImmediate(objectsToDestroy.Buffer[i]);
            }

            objectsToDestroy.Clear();
            objectsToDestroyInstanceIds.Clear();
        }
    }

    #endregion

    /// <summary>
    /// A unique identifier for this object.
    /// </summary>
    /// <remarks>This ID is <b>not</b> guaranteed to be the same between sessions.</remarks>
    public uint InstanceID { get; private set; }

    bool isDestroyed = false;

    internal BlueObject() {
        InstanceID = nextInstanceId++;
    }

    protected abstract void Destroy();

    public override int GetHashCode() {
        return InstanceID.GetHashCode();
    }

    #region Equality

    public static bool operator ==(BlueObject obj1, BlueObject obj2) {
        bool obj1Null = obj1 is null || obj1.isDestroyed;
        bool obj2Null = obj2 is null || obj2.isDestroyed;

        if (obj1Null && obj2Null) {
            return true;
        }

        if ((obj1Null && !obj2Null) || (obj2Null && !obj1Null)) {
            return false;
        }

        return obj1.InstanceID == obj2.InstanceID;
    }

    public static bool operator !=(BlueObject obj1, BlueObject obj2) {
        return !(obj1 == obj2);
    }

    public bool Equals(BlueObject obj) {
        return this == obj;
    }

    public override bool Equals(object obj) {
        return obj is BlueObject blObj && this == blObj;
    }

    #endregion
}
