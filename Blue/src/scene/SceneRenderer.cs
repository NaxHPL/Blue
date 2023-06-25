using BlueFw.Extensions;
using BlueFw.Math;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BlueFw;

internal class SceneRenderer : SceneHandler<IRenderable> {

    static readonly IComparer<IRenderable> renderableComparer = new RenderableOrderComparer();
    protected override IComparer<IRenderable> itemComparer => renderableComparer;

    public void Render(SpriteBatch spriteBatch, Camera camera) {
        PrepareItemsForHandling();

        Blue.Instance.GraphicsDevice.Clear(camera.ClearColor);

        bool startedFirstBatch = false;
        Material currentMaterial = null;

        for (int i = 0; i < items.Length; i++) {
            IRenderable renderable = items.Buffer[i];

            // Don't render if inactive
            if (!renderable.Active) {
                continue;
            }

            // Don't render if the camera can't see it
            Rect.Overlaps(renderable.Bounds, camera.Bounds, out bool seenByCamera);
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