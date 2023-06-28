using BlueFw.Math;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BlueFw;

internal class SceneRenderer : SceneHandler<IRenderable> {

    static readonly IComparer<IRenderable> renderableComparer = new RenderableOrderComparer();
    protected override IComparer<IRenderable> itemComparer => renderableComparer;

    public void Render(BlueSpriteBatch spriteBatch, Camera camera) {
        PrepareItemsForHandling();

        Blue.Instance.GraphicsDevice.Clear(camera.ClearColor);

        Rectangle viewportBounds = Blue.Instance.GraphicsDevice.Viewport.Bounds;
        Rectangle cameraBounds = camera.Bounds;

        bool hasStartedBatch = false; // did we start a batch at all? if not, we don't need to call spriteBatch.End()
        Material currentMaterial = null;
        bool currentlyRenderingInScreenSpace = false;

        for (int i = 0; i < items.Length; i++) {
            IRenderable renderable = items.Buffer[i];

            // Don't render if inactive
            if (!renderable.Active) {
                continue;
            }

            // Don't render if it can't be seen
            Rect.Overlaps(renderable.Bounds, renderable.RenderInScreenSpace ? viewportBounds : cameraBounds, out bool isInBounds);
            if (!isInBounds) {
                continue;
            }

            if (hasStartedBatch) {
                EnsureStateForRenderable(renderable, currentMaterial, currentlyRenderingInScreenSpace, spriteBatch, camera);
            }
            else {
                currentMaterial = renderable.Material ?? Material.Default;
                spriteBatch.Begin(currentMaterial, renderable.RenderInScreenSpace ? null : camera.TransformMatrix);
                hasStartedBatch = true;
            }

            currentlyRenderingInScreenSpace = renderable.RenderInScreenSpace;

            renderable.Render(spriteBatch, camera);
        }

        if (hasStartedBatch) {
            spriteBatch.End();
        }
    }

    static void EnsureStateForRenderable(IRenderable renderable, Material currentMaterial, bool currentlyRenderingInScreenSpace, BlueSpriteBatch spriteBatch, Camera camera) {
        Material material = currentMaterial;
        bool shouldFlush = false;
        
        if (renderable.Material == null && currentMaterial != Material.Default) {
            material = Material.Default;
            shouldFlush = true;
        }
        else if (renderable.Material != null && currentMaterial != renderable.Material) {
            material = renderable.Material;
            shouldFlush = true;
        }
        else if (currentlyRenderingInScreenSpace != renderable.RenderInScreenSpace) {
            shouldFlush = true;
        }

        if (shouldFlush) {
            spriteBatch.Flush(material, renderable.RenderInScreenSpace ? null : camera.TransformMatrix);
        }
    }
}