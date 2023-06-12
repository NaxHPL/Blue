using Microsoft.Xna.Framework.Content.Pipeline;
using System.Text.Json.Nodes;

namespace BlueContentPipeline;

[ContentProcessor(DisplayName = "SDF Font Processor - Blue")]
public class SDFFontProcessor : ContentProcessor<SDFFontRawContent, SDFFontContent> {

    public override SDFFontContent Process(SDFFontRawContent rawContent, ContentProcessorContext context) {
        SDFFontContent output = new SDFFontContent();

        JsonNode? layoutDataRoot = JsonNode.Parse(rawContent.LayoutDataJson) ?? throw new Exception("Unable to parse layout data json.");

        output.FontName = layoutDataRoot["name"]!.GetValue<string>();

        JsonNode? atlasData = layoutDataRoot["atlas"] ?? throw new Exception("No atlas layout data found.");
        output.AtlasWidth = atlasData["width"]!.GetValue<int>();
        output.AtlasHeight = atlasData["height"]!.GetValue<int>();
        output.Distances = rawContent.AtlasData;

        JsonNode? metricsData = layoutDataRoot["metrics"] ?? throw new Exception("No metrics data found.");
        output.LineHeight = metricsData["lineHeight"]!.GetValue<float>();
        output.Ascender = metricsData["ascender"]!.GetValue<float>();
        output.Descender = metricsData["descender"]!.GetValue<float>();
        output.UnderlineY = metricsData["underlineY"]!.GetValue<float>();
        output.UnderlineThickness = metricsData["underlineThickness"]!.GetValue<float>();

        JsonNode? glyphsData = layoutDataRoot["glyphs"] ?? throw new Exception("No glyph data found.");
        output.Glyphs = glyphsData.AsArray().Select(node => new GlyphContent(node)).ToArray();

        JsonNode? kerningData = layoutDataRoot["kerning"] ?? throw new Exception("No kerning data found.");
        output.KerningPairs = kerningData.AsArray().Select(node => new KerningPair(node)).ToArray();

        return output;
    }
}
