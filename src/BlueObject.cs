namespace Blue;

/// <summary>
/// Base class for all Blue objects.
/// </summary>
public class BlueObject {

    /// <summary>
    /// A unique identifier for this object.
    /// </summary>
    /// <remarks>This ID is <b>not</b> guaranteed to be the same between sessions.</remarks>
    public uint InstanceID { get; private set; }

    static uint nextUniqueId = 0;

    public BlueObject() {
        InstanceID = nextUniqueId++;
    }
}
