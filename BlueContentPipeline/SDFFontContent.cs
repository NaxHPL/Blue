using Microsoft.Xna.Framework.Content.Pipeline;

namespace BlueContentPipeline;

public class SDFFontContent : ContentItem {

    public string FontName = "Unknown";
    public int AtlasWidth;
    public int AtlasHeight;
    public byte[] Distances = Array.Empty<byte>();
    public float LineHeight;
    public float Ascender;
    public float Descender;
    public float UnderlineY;
    public float UnderlineThickness;
    public GlyphContent[] Glyphs = Array.Empty<GlyphContent>();
    public KerningPair[] KerningPairs = Array.Empty<KerningPair>();
}
