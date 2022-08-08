using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BlueFw;

internal class SceneRenderer {

    static readonly IComparer<IRenderable> renderableComparer = new RenderableOrderComparer();

    readonly Scene scene;

    readonly FastList<IRenderable> renderables = new FastList<IRenderable>();
    readonly HashSet<IRenderable> renderablesSet = new HashSet<IRenderable>();

    readonly HashSet<IRenderable> renderablesPendingAdd = new HashSet<IRenderable>();
    readonly HashSet<IRenderable> renderablesPendingRemove = new HashSet<IRenderable>();

    bool renderableOrderDirty = true;

    public SceneRenderer(Scene scene) {
        this.scene = scene;
    }

    public bool Register(IRenderable renderable) {
        if (renderable == null) {
            return false;
        }

        if (renderablesSet.Contains(renderable)) {
            return false;
        }

        if (renderablesPendingRemove.Remove(renderable)) {
            return true;
        }

        return renderablesPendingAdd.Add(renderable);
    }

    public bool Unregister(IRenderable renderable) {
        if (renderable == null) {
            return false;
        }

        if (!renderablesSet.Contains(renderable)) {
            return false;
        }

        if (renderablesPendingAdd.Remove(renderable)) {
            return true;
        }

        return renderablesPendingRemove.Add(renderable);
    }

    public void FlagRenderOrderDirty() {
        renderableOrderDirty = true;
    }

    public void Render(SpriteBatch spriteBatch) {
        ApplyPendingChanges();

        // Render
    }

    void ApplyPendingChanges() {
        if (renderablesPendingAdd.Count > 0) {
            foreach (IRenderable renderable in renderablesPendingAdd) {
                Add(renderable);
            }
        }

        if (renderablesPendingRemove.Count > 0) {
            foreach (IRenderable renderable in renderablesPendingRemove) {
                Remove(renderable);
            }
        }

        if (renderableOrderDirty) {
            renderables.Sort(renderableComparer);
            renderableOrderDirty = false;
        }
    }

    void Add(IRenderable renderable) {
        renderables.Add(renderable);
        renderablesSet.Add(renderable);
        renderableOrderDirty = true;
    }

    void Remove(IRenderable renderable) {
        renderables.Remove(renderable);
        renderablesSet.Remove(renderable);
        renderableOrderDirty = true;
    }
}
