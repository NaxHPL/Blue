using BlueFw.Extensions;
using BlueFw.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BlueFw;

internal class SceneScreenRenderer : SceneHandler<IScreenRenderable> {

    static readonly IComparer<IScreenRenderable> screenRenderableComparer = new ScreenRenderableOrderComparer();
    protected override IComparer<IScreenRenderable> itemComparer => screenRenderableComparer;

    public void Render(SpriteBatch spriteBatch) {
        PrepareItemsForHandling();

        bool startedFirstBatch = false;
        Material currentMaterial = null;

        Rectangle viewportBounds = Blue.Instance.GraphicsDevice.Viewport.Bounds;

        for (int i = 0; i < items.Length; i++) {
            IScreenRenderable renderable = items.Buffer[i];

            // Don't render if inactive
            if (!renderable.Active) {
                continue;
            }

            // Don't render if the viewport can't see it
            Rect.Overlaps(renderable.Bounds, viewportBounds, out bool seenByCamera);
            if (!seenByCamera) {
                continue;
            }

            if (startedFirstBatch) {
                EnsureStateForRenderable(renderable, currentMaterial, spriteBatch);
            }
            else {
                currentMaterial = renderable.Material ?? Material.Default;
                spriteBatch.Begin(currentMaterial);
                startedFirstBatch = true;
            }

            renderable.Render(spriteBatch);
        }

        if (startedFirstBatch) {
            spriteBatch.End();
        }
    }

    static void EnsureStateForRenderable(IScreenRenderable renderable, Material currentMaterial, SpriteBatch spriteBatch) {
        if (renderable.Material == null && currentMaterial != Material.Default) {
            currentMaterial = Material.Default;
            spriteBatch.Flush(currentMaterial);
        }
        else if (renderable.Material != null && currentMaterial != renderable.Material) {
            currentMaterial = renderable.Material;
            spriteBatch.Flush(currentMaterial);
        }
    }
}
