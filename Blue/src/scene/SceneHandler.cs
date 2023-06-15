using System.Collections.Generic;

namespace BlueFw;

internal abstract class SceneHandler<T> {

    protected abstract IComparer<T> itemComparer { get; }

    protected readonly FastList<T> items = new FastList<T>();

    readonly HashSet<T> itemsSet = new HashSet<T>();
    readonly HashSet<T> itemsPendingAdd = new HashSet<T>();
    readonly HashSet<T> itemsPendingRemove = new HashSet<T>();

    bool itemOrderDirty = true;

    public bool Register(T item) {
        if (item == null) {
            return false;
        }

        if (itemsSet.Contains(item)) {
            return false;
        }

        if (itemsPendingRemove.Remove(item)) {
            return true;
        }

        return itemsPendingAdd.Add(item);
    }

    public bool Unregister(T item) {
        if (item == null) {
            return false;
        }

        if (!itemsSet.Contains(item)) {
            return false;
        }

        if (itemsPendingAdd.Remove(item)) {
            return true;
        }

        return itemsPendingRemove.Add(item);
    }

    public void FlagItemOrderDirty() {
        itemOrderDirty = true;
    }

    void ApplyPendingChanges() {
        if (itemsPendingAdd.Count > 0) {
            foreach (T item in itemsPendingAdd) {
                items.Add(item);
                itemsSet.Add(item);
            }

            itemsPendingAdd.Clear();
            itemOrderDirty = true;
        }

        if (itemsPendingRemove.Count > 0) {
            foreach (T item in itemsPendingRemove) {
                items.Remove(item);
                itemsSet.Remove(item);
            }

            itemsPendingRemove.Clear();
        }
    }

    void EnsureItemsSorted() {
        if (!itemOrderDirty) {
            return;
        }

        items.Sort(itemComparer);
        itemOrderDirty = false;
    }

    protected void PrepareItemsForHandling() {
        ApplyPendingChanges();
        EnsureItemsSorted();
    }
}
