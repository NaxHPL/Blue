using System;
using System.Collections.Generic;

namespace BlueFw;

internal class EntityCollection {

    /// <summary>
    /// The scene this <see cref="EntityCollection"/> is attached to.
    /// </summary>
    public readonly Scene Scene;

    /// <summary>
    /// The number of entities in this collection.
    /// </summary>
    public int Count => allEntities.Length;

    /// <summary>
    /// Gets the <see cref="Entity"/> at the specified index.
    /// </summary>
    public Entity this[int index] {
        get {
            if (index >= allEntities.Length) {
                throw new IndexOutOfRangeException();
            }

            return allEntities.Buffer[index];
        }
    }

    readonly FastList<Entity> allEntities = new FastList<Entity>();
    readonly HashSet<uint> allEntitiesInstanceIds = new HashSet<uint>(); // More efficient to search this instead of the allEntities list, but gotta keep them in sync
    readonly HashSet<Entity> entitiesToAdd = new HashSet<Entity>();
    readonly HashSet<Entity> entitiesToRemove = new HashSet<Entity>();

    static readonly List<Entity> tempEntityList = new List<Entity>(); // A reusable list used by various methods
    static readonly List<Component> tempComponentList = new List<Component>(); // A reusable list used by various methods

    /// <summary>
    /// Creates a new <see cref="EntityCollection"/>.
    /// </summary>
    /// <param name="scene">The scene that owns this collection.</param>
    public EntityCollection(Scene scene) {
        Scene = scene;
    }

    /// <summary>
    /// Adds an entity.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="entity"/> wasn't already in this collection and added successfully; otherwise <see langword="false"/>.
    /// </returns>
    public bool Add(Entity entity) {
        if (entity == null) {
            return false;
        }

        if (entitiesToRemove.Remove(entity)) {
            return true;
        }

        if (allEntitiesInstanceIds.Contains(entity.InstanceID)) {
            return false;
        }

        return entitiesToAdd.Add(entity);
    }

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <paramref name="entity"/> was found and removed from the collection; otherwise <see langword="false"/>.
    /// </returns>
    public bool Remove(Entity entity) {
        if (entity == null) {
            return false;
        }

        if (entitiesToAdd.Remove(entity)) {
            return true;
        }

        if (!allEntitiesInstanceIds.Contains(entity.InstanceID)) {
            return false;
        }

        return entitiesToRemove.Add(entity);
    }

    /// <summary>
    /// Returns <see langword="true"/> if this <see cref="EntityCollection"/> contains <paramref name="entity"/>.
    /// </summary>
    public bool Contains(Entity entity) {
        return
            !entitiesToRemove.Contains(entity) &&
            (entitiesToAdd.Contains(entity) || allEntitiesInstanceIds.Contains(entity.InstanceID));
    }

    /// <summary>
    /// Finds an entity in this collection with the specified name.
    /// </summary>
    public Entity Find(string name) {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (entity.Name == name && !entitiesToRemove.Contains(entity)) {
                return entity;
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            if (entity.Name == name) {
                return entity;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds an entity in this collection with the specified instance ID.
    /// </summary>
    public Entity Find(uint instanceId) {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (entity.InstanceID == instanceId && !entitiesToRemove.Contains(entity)) {
                return entity;
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            if (entity.InstanceID == instanceId) {
                return entity;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds an entity of type <typeparamref name="T"/>.
    /// </summary>
    public T Find<T>() where T : Entity {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (entity is T e && !entitiesToRemove.Contains(entity)) {
                return e;
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            if (entity is T e) {
                return e;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/> and adds them to <paramref name="results"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for entities which are active in the hierarchy.
    /// Set <paramref name="includeInactive"/> to <see langword="true"/> to include inactive entities in the search.
    /// </remarks>
    /// <param name="includeInactive">(Optional) Include inactive entities in the search.</param>
    public void FindAll<T>(List<T> results, bool includeInactive = false) where T : Entity {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (entity is T e && (includeInactive || entity.ActiveInHierarchy) && !entitiesToRemove.Contains(entity)) {
                results.Add(e);
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            if (entity is T e && (includeInactive || entity.ActiveInHierarchy)) {
                results.Add(e);
            }
        }
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for entities which are active in the hierarchy.
    /// Set <paramref name="includeInactive"/> to <see langword="true"/> to include inactive entities in the search.
    /// </remarks>
    /// <param name="includeInactive">(Optional) Include inactive entities in the search.</param>
    public T[] FindAll<T>(bool includeInactive = false) where T : Entity {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (entity is T && (includeInactive || entity.ActiveInHierarchy) && !entitiesToRemove.Contains(entity)) {
                tempEntityList.Add(entity);
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            if (entity is T && (includeInactive || entity.ActiveInHierarchy)) {
                tempEntityList.Add(entity);
            }
        }

        T[] arr = (T[])tempEntityList.ToArray();
        tempEntityList.Clear();
        return arr;
    }

    /// <summary>
    /// Finds a component of type <typeparamref name="T"/> attached to the entites in this collection.
    /// </summary>
    public T FindComponent<T>() where T : Component {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (!entitiesToRemove.Contains(entity) && entity.TryGetComponent(out T c)) {
                return c;
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            if (entity.TryGetComponent(out T c)) {
                return c;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> in the scene and adds them to <paramref name="results"/>.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for components which are active in the hierarchy.
    /// Set <paramref name="includeInactive"/> to <see langword="true"/> to include inactive components in the search.
    /// </remarks>
    /// <param name="includeInactive">(Optional) Include inactive components in the search.</param>
    public void FindComponents<T>(List<T> results, bool includeInactive = false) where T : Component {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (!entitiesToRemove.Contains(entity)) {
                entity.GetComponents(results, includeInactive);
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            entity.GetComponents(results, includeInactive);
        }
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> in the scene.
    /// </summary>
    /// <remarks>
    /// By default, this only searches for components which are active in the hierarchy.
    /// Set <paramref name="includeInactive"/> to <see langword="true"/> to include inactive components in the search.
    /// </remarks>
    /// <param name="includeInactive">(Optional) Include inactive components in the search.</param>
    public T[] FindComponents<T>(bool includeInactive = false) where T : Component {
        for (int i = 0; i < allEntities.Length; i++) {
            Entity entity = allEntities.Buffer[i];
            if (!entitiesToRemove.Contains(entity)) {
                entity.GetComponents(tempComponentList, includeInactive);
            }
        }

        foreach (Entity entity in entitiesToAdd) {
            entity.GetComponents(tempComponentList, includeInactive);
        }

        T[] arr = (T[])tempComponentList.ToArray();
        tempComponentList.Clear();
        return arr;
    }

    // TODO: CALL THIS AT END OF UPDATE
    internal void ApplyPendingChanges() {
        if (entitiesToRemove.Count > 0) {
            foreach (Entity entity in entitiesToRemove) {
                RemoveImmediate(entity);
            }
            entitiesToRemove.Clear();
        }

        if (entitiesToAdd.Count > 0) {
            foreach (Entity entity in entitiesToAdd) {
                AddImmediate(entity);
            }
            entitiesToAdd.Clear();
        }
    }

    void RemoveImmediate(Entity entity) {
        allEntities.Remove(entity);
        allEntitiesInstanceIds.Remove(entity.InstanceID);
    }

    void AddImmediate(Entity entity) {
        allEntities.Add(entity);
        allEntitiesInstanceIds.Add(entity.InstanceID);
    }
}
