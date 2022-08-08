using System.Collections.Generic;

namespace BlueFw;

internal interface IUpdatable {

    /// <summary>
    /// Defines whether this updatable should be updated.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Defines when this updatable will be updated.
    /// </summary>
    /// <remarks>
    /// If you plan to change this value after this updatable is added to a scene,
    /// you must call <see cref="Scene.ApplyUpdateOrderChanges"/> after this value changes.
    /// </remarks>
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
