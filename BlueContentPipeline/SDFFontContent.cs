using Microsoft.Xna.Framework.Content.Pipeline;
using System.Drawing;

namespace BlueContentPipeline;

public class SDFFontContent : ContentItem {

    public uint AtlasWidth;
    public uint AtlasHeight;
    public Color[] AtlasPixelData = Array.Empty<Color>();
    public string LayoutDataJson = "{}";
}
