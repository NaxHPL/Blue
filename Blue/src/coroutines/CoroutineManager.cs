using BlueFw.Utils;
using System.Collections.Generic;

namespace BlueFw.Coroutines;

internal class CoroutineManager {

    readonly FastList<Coroutine> coroutines = new FastList<Coroutine>();

    readonly HashSet<Coroutine> coroutinesSet = new HashSet<Coroutine>();
    readonly HashSet<Coroutine> pendingAdd = new HashSet<Coroutine>();
    readonly HashSet<Coroutine> pendingRemove = new HashSet<Coroutine>();

    internal bool Register(Coroutine coroutine) {
        if (coroutine == null) {
            return false;
        }

        if (coroutinesSet.Contains(coroutine)) {
            return false;
        }

        if (pendingRemove.Remove(coroutine)) {
            return true;
        }

        return pendingAdd.Add(coroutine);
    }

    internal bool Unregister(Coroutine coroutine) {
        if (coroutine == null) {
            return false;
        }

        if (!coroutinesSet.Contains(coroutine)) {
            return false;
        }

        if (pendingAdd.Remove(coroutine)) {
            return true;
        }

        return pendingRemove.Add(coroutine);
    }

    internal void Update() {
        ApplyPendingChanges();

        for (int i = 0; i < coroutines.Length; i++) {
            Coroutine cr = coroutines.Buffer[i];

            // If it's been unregistered, just skip
            if (pendingRemove.Contains(cr)) {
                continue;
            }

            if (cr.Advance()) {
                pendingRemove.Add(cr);
            }
        }
    }

    void ApplyPendingChanges() {
        if (pendingAdd.Count > 0) {
            foreach (Coroutine cr in pendingAdd) {
                coroutines.Add(cr);
                coroutinesSet.Add(cr);
            }

            pendingAdd.Clear();
        }

        if (pendingRemove.Count > 0) {
            foreach (Coroutine cr in pendingRemove) {
                coroutines.Remove(cr);
                coroutinesSet.Remove(cr);
                cr.Release();
            }

            pendingRemove.Clear();
        }
    }
}
