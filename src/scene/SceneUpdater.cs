﻿using System.Collections.Generic;

namespace BlueFw;

internal class SceneUpdater {

    static readonly IComparer<IUpdatable> updatableComparer = new UpdatableOrderComparer();

    readonly FastList<IUpdatable> updatables = new FastList<IUpdatable>();
    readonly HashSet<IUpdatable> updatablesSet = new HashSet<IUpdatable>();

    readonly HashSet<IUpdatable> updatablesPendingAdd = new HashSet<IUpdatable>();
    readonly HashSet<IUpdatable> updatablesPendingRemove = new HashSet<IUpdatable>();

    bool updateOrderDirty = true;

    public bool Register(IUpdatable updatable) {
        if (updatable == null) {
            return false;
        }

        if (updatablesSet.Contains(updatable)) {
            return false;
        }

        if (updatablesPendingRemove.Remove(updatable)) {
            return true;
        }

        return updatablesPendingAdd.Add(updatable);
    }

    public bool Unregister(IUpdatable updatable) {
        if (updatable == null) {
            return false;
        }

        if (!updatablesSet.Contains(updatable)) {
            return false;
        }

        if (updatablesPendingAdd.Remove(updatable)) {
            return true;
        }

        return updatablesPendingRemove.Add(updatable);
    }

    public void FlagUpdateOrderDirty() {
        updateOrderDirty = true;
    }

    public void Update() {
        ApplyPendingChanges();
        EnsureUpdatablesSorted();

        for (int i = 0; i < updatables.Length; i++) {
            if (updatables.Buffer[i].Active) {
                updatables.Buffer[i].Update();
            }
        }
    }

    void ApplyPendingChanges() {
        if (updatablesPendingAdd.Count > 0) {
            foreach (IUpdatable updatable in updatablesPendingAdd) {
                updatables.Add(updatable);
                updatablesSet.Add(updatable);
            }

            updatablesPendingAdd.Clear();
            updateOrderDirty = true;
        }

        if (updatablesPendingRemove.Count > 0) {
            foreach (IUpdatable updatable in updatablesPendingRemove) {
                updatables.Remove(updatable);
                updatablesSet.Remove(updatable);
            }

            updatablesPendingRemove.Clear();
        }
    }

    void EnsureUpdatablesSorted() {
        if (!updateOrderDirty) {
            return;
        }

        updatables.Sort(updatableComparer);
        updateOrderDirty = false;
    }
}
