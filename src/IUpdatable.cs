using System.Collections.Generic;

namespace Blue;

internal interface IUpdatable {

    /// <summary>
    /// Defines whether this updatable should be updated.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Defines when this updatable will be updated.
    /// </summary>
    int UpdateOrder { get; }

    /// <summary>
    /// Updates this updatable.
    /// </summary>
    void Update();
}

internal class UpdatableOrderComparer : IComparer<IUpdatable> {

    public int Compare(IUpdatable a, IUpdatable b) {
        return a.UpdateOrder.CompareTo(b.UpdateOrder);
    }
}
