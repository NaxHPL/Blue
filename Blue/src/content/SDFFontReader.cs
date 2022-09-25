using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BlueFw.Content;

internal class SDFFontReader : ContentTypeReader<SDFFont> {

    protected override SDFFont Read(ContentReader reader, SDFFont existingInstance) {
        int atlasWidth = reader.ReadInt32();
        int atlasHeight = reader.ReadInt32();

        Color[] pixelColorData = new Color[atlasWidth * atlasHeight];
        for (int y = 0; y < atlasHeight; y++) {
            for (int x = 0; x < atlasWidth; x++) {
                pixelColorData[y * atlasWidth + x] = reader.ReadColor();
            }
        }

        string layoutDataJson = reader.ReadString();

        Texture2D atlasTexture = new Texture2D(Blue.Instance.GraphicsDevice, atlasWidth, atlasHeight);
        atlasTexture.SetData(pixelColorData);

        return new SDFFont();
    }
}
