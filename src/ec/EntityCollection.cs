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
    public int Count => entities.Length;

    /// <summary>
    /// Gets the <see cref="Entity"/> at the specified index.
    /// </summary>
    public Entity this[int index] {
        get {
            if (index >= entities.Length) {
                throw new IndexOutOfRangeException();
            }

            return entities.Buffer[index];
        }
    }

    readonly FastList<Entity> entities = new FastList<Entity>();
    readonly HashSet<uint> entityInstanceIds = new HashSet<uint>(); // Used for checking if the collection contains an entity

    static readonly List<Entity> reusableEntityList = new List<Entity>();
    static readonly List<Component> reusableComponentList = new List<Component>();

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

        if (!entityInstanceIds.Add(entity.InstanceID)) {
            return false;
        }

        entities.Add(entity);
        return true;
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

        if (!entityInstanceIds.Remove(entity.InstanceID)) {
            return false;
        }

        entities.Remove(entity);
        return true;
    }

    /// <summary>
    /// Returns <see langword="true"/> if this <see cref="EntityCollection"/> contains <paramref name="entity"/>.
    /// </summary>
    public bool Contains(Entity entity) {
        return entity != null && entityInstanceIds.Contains(entity.InstanceID);
    }

    /// <summary>
    /// Finds an entity in this collection with the specified name.
    /// Returns null if not found.
    /// </summary>
    public Entity Find(string name) {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i].Name == name) {
                return entities.Buffer[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Finds an entity in this collection with the specified instance ID.
    /// Returns null if not found.
    /// </summary>
    public Entity Find(uint instanceId) {
        if (!entityInstanceIds.Contains(instanceId)) {
            return null;
        }

        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i].InstanceID == instanceId) {
                return entities.Buffer[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Finds an entity in this collection of type <typeparamref name="T"/>.
    /// Returns null if not found.
    /// </summary>
    public T Find<T>() where T : Entity {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i] is T e) {
                return e;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/> and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider entities which are active in the hierarchy.</param>
    public void FindAll<T>(List<T> results, bool onlyActive = false) where T : Entity {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i] is T e && (!onlyActive || e.Active)) {
                results.Add(e);
            }
        }
    }

    /// <summary>
    /// Finds all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider entities which are active in the hierarchy.</param>
    public T[] FindAll<T>(bool onlyActive = false) where T : Entity {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i] is T && (!onlyActive || entities.Buffer[i].Active)) {
                reusableEntityList.Add(entities.Buffer[i]);
            }
        }

        T[] arr = (T[])reusableEntityList.ToArray();
        reusableEntityList.Clear();
        return arr;
    }

    /// <summary>
    /// Finds a component of type <typeparamref name="T"/> attached to the entites in this collection.
    /// Returns null if not found.
    /// </summary>
    public T FindComponent<T>() where T : Component {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i].TryGetComponent(out T c)) {
                return c;
            }
        }

        return null;
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> in the scene and adds them to <paramref name="results"/>.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public void FindComponents<T>(List<T> results, bool onlyActive = false) where T : Component {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i].TryGetComponent(out T c) && (!onlyActive || c.Active)) {
                results.Add(c);
            }
        }
    }

    /// <summary>
    /// Finds all components of type <typeparamref name="T"/> in the scene.
    /// </summary>
    /// <param name="onlyActive">(Optional) Only consider components which are active in the hierarchy.</param>
    public T[] FindComponents<T>(bool onlyActive = false) where T : Component {
        for (int i = 0; i < entities.Length; i++) {
            if (entities.Buffer[i].TryGetComponent(out T c) && (!onlyActive || c.Active)) {
                reusableComponentList.Add(c);
            }
        }

        T[] arr = (T[])reusableComponentList.ToArray();
        reusableComponentList.Clear();
        return arr;
    }
}
