using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BlueContentPipeline;

public static class ContentWriterExt {

    public static void Write(this ContentWriter writer, Bounds bounds) {
        writer.Write(bounds.Top);
        writer.Write(bounds.Right);
        writer.Write(bounds.Bottom);
        writer.Write(bounds.Left);
    }

    public static void Write(this ContentWriter writer, GlyphContent glyph) {
        writer.Write(glyph.Codepoint);
        writer.Write(glyph.Advance);
        writer.Write(glyph.PlaneBounds);
        writer.Write(glyph.AtlasBounds);
    }

    public static void Write(this ContentWriter writer, KerningPair kerningPair) {
        writer.Write(kerningPair.Codepoint1);
        writer.Write(kerningPair.Codepoint2);
        writer.Write(kerningPair.Advance);
    }
}
