﻿using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BlueFw;

internal class SceneRenderer {

    static readonly IComparer<IRenderable> renderableComparer = new RenderableOrderComparer();

    readonly FastList<IRenderable> renderables = new FastList<IRenderable>();
    readonly HashSet<IRenderable> renderablesSet = new HashSet<IRenderable>();

    readonly HashSet<IRenderable> renderablesPendingAdd = new HashSet<IRenderable>();
    readonly HashSet<IRenderable> renderablesPendingRemove = new HashSet<IRenderable>();

    bool renderableOrderDirty = true;

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

    void EnsureRenderablesSorted() {
        if (!renderableOrderDirty) {
            return;
        }

        renderables.Sort(renderableComparer);
        renderableOrderDirty = false;
    }

    void ApplyPendingChanges() {
        if (renderablesPendingAdd.Count > 0) {
            foreach (IRenderable renderable in renderablesPendingAdd) {
                renderables.Add(renderable);
                renderablesSet.Add(renderable);
            }

            renderablesPendingAdd.Clear();
            renderableOrderDirty = true;
        }

        if (renderablesPendingRemove.Count > 0) {
            foreach (IRenderable renderable in renderablesPendingRemove) {
                renderables.Remove(renderable);
                renderablesSet.Remove(renderable);
            }

            renderablesPendingRemove.Clear();
        }
    }

    public void Render(SpriteBatch spriteBatch, Camera camera) {
        ApplyPendingChanges();
        EnsureRenderablesSorted();

        Blue.Instance.GraphicsDevice.Clear(camera.ClearColor);

        bool seenByCamera;
        bool startedFirstBatch = false;
        Material currentMaterial = null;

        for (int i = 0; i < renderables.Length; i++) {
            IRenderable renderable = renderables.Buffer[i];

            // Don't render if inactive
            if (!renderable.Active) {
                continue;
            }

            // Don't render if the camera can't see it
            Rect.Overlaps(renderable.Bounds, camera.Bounds, out seenByCamera);
            if (!seenByCamera) {
                continue;
            }

            if (startedFirstBatch) {
                EnsureStateForRenderable(renderable, currentMaterial, spriteBatch, camera);
            }
            else {
                currentMaterial = renderable.Material ?? Material.Default;
                spriteBatch.Begin(currentMaterial, camera.TransformMatrix);
                startedFirstBatch = true;
            }

            renderable.Render(spriteBatch, camera);
        }

        if (startedFirstBatch) {
            spriteBatch.End();
        }
    }

    static void EnsureStateForRenderable(IRenderable renderable, Material currentMaterial, SpriteBatch spriteBatch, Camera camera) {
        if (renderable.Material == null && currentMaterial != Material.Default) {
            currentMaterial = Material.Default;
            spriteBatch.Flush(currentMaterial, camera.TransformMatrix);
        }
        else if (renderable.Material != null && currentMaterial != renderable.Material) {
            currentMaterial = renderable.Material;
            spriteBatch.Flush(currentMaterial, camera.TransformMatrix);
        }
    }
}