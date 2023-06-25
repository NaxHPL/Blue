using Microsoft.Xna.Framework.Content;

namespace BlueFw.Content;

public class SDFFontReader : ContentTypeReader<SDFFont> {

    protected override SDFFont Read(ContentReader reader, SDFFont existingInstance) {
        return null;

        //int atlasWidth = reader.ReadInt32();
        //int atlasHeight = reader.ReadInt32();

        //Color[] pixelColorData = new Color[atlasWidth * atlasHeight];
        //for (int i = 0; i < pixelColorData.Length; i++) {
        //    byte c = reader.ReadByte();
        //    pixelColorData[i] = new Color(c, c, c, c);
        //}

        //string layoutDataJson = reader.ReadString();

        //Texture2D atlasTexture = new Texture2D(Blue.Instance.GraphicsDevice, atlasWidth, atlasHeight);
        //atlasTexture.SetData(pixelColorData);

        //return new SDFFont() {
        //    AtlasTexture = atlasTexture
        //};
    }
}
