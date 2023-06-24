using BlueFw.Utils;
using System.Collections.Generic;

namespace BlueFw.Coroutines;

internal static class CoroutineUpdater {

    static readonly FastList<Coroutine> coroutines = new FastList<Coroutine>();

    static readonly HashSet<Coroutine> coroutinesSet = new HashSet<Coroutine>();
    static readonly HashSet<Coroutine> pendingAdd = new HashSet<Coroutine>();
    static readonly HashSet<Coroutine> pendingRemove = new HashSet<Coroutine>();

    internal static bool Register(Coroutine coroutine) {
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

    internal static bool Unregister(Coroutine coroutine) {
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

    internal static void Update() {
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

    static void ApplyPendingChanges() {
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
