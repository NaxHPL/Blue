namespace BlueFw;

/// <summary>
/// Base class for all Blue objects.
/// </summary>
public abstract class BlueObject {

    /// <summary>
    /// A unique identifier for this object.
    /// </summary>
    /// <remarks>This ID is <b>not</b> guaranteed to be the same between sessions.</remarks>
    public uint InstanceID { get; private set; }

    static uint nextUniqueId = 0;

    internal BlueObject() {
        InstanceID = nextUniqueId++;
    }

    public abstract void Destroy();

    public static bool operator ==(BlueObject obj1, BlueObject obj2) {
        bool obj1Null = ReferenceEquals(obj1, null);
        bool obj2Null = ReferenceEquals(obj2, null);

        if (obj1Null && obj2Null) {
            return true;
        }
        else if ((obj1Null && !obj2Null) || (obj2Null && !obj1Null)) {
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

    public override int GetHashCode() {
        return InstanceID.GetHashCode();
    }
}
