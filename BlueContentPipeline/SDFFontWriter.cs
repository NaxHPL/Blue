using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BlueContentPipeline;

[ContentTypeWriter]
public class SDFFontWriter : ContentTypeWriter<SDFFontContent> {

    protected override void Write(ContentWriter writer, SDFFontContent fontContent) {
        writer.Write(fontContent.FontName);
        writer.Write(fontContent.AtlasWidth);
        writer.Write(fontContent.AtlasHeight);
        writer.Write(fontContent.Distances);
        writer.Write(fontContent.LineHeight);
        writer.Write(fontContent.Ascender);
        writer.Write(fontContent.Descender);
        writer.Write(fontContent.UnderlineY);
        writer.Write(fontContent.UnderlineThickness);

        writer.Write(fontContent.Glyphs.Length);
        foreach (GlyphContent glyph in fontContent.Glyphs) {
            writer.Write(glyph);
        }

        writer.Write(fontContent.KerningPairs.Length);
        foreach (KerningPair kerningPair in fontContent.KerningPairs) {
            writer.Write(kerningPair);
        }
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform) {
        return "BlueFw.Content.SDFFontReader, Blue";
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform) {
        return "BlueFw.Content.SDFFont, Blue";
    }
}
