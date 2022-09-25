using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace BlueContentPipeline;

public class SDFFontContent : ContentItem {

    public int AtlasWidth;
    public int AtlasHeight;
    public byte[] AtlasPixelData = Array.Empty<byte>();
    public string LayoutDataJson = "{}";
}
