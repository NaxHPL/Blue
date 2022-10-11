using Microsoft.Xna.Framework.Content.Pipeline;
using System.Text.Json.Nodes;

namespace BlueContentPipeline;

[ContentProcessor(DisplayName = "SDF Font Processor - Blue")]
public class SDFFontProcessor : ContentProcessor<SDFFontRawContent, SDFFontContent> {

    public override SDFFontContent Process(SDFFontRawContent rawContent, ContentProcessorContext context) {
        SDFFontContent output = new SDFFontContent();

        JsonNode? layoutDataRoot = JsonNode.Parse(rawContent.LayoutDataJson);
        if (layoutDataRoot == null) {
            throw new Exception("Unable to parse layout data json.");
        }

        output.FontName = layoutDataRoot["name"]!.GetValue<string>();

        JsonNode? atlasData = layoutDataRoot["atlas"];
        if (atlasData == null) {
            throw new Exception("No atlas layout data found.");
        }

        output.AtlasWidth = atlasData["width"]!.GetValue<int>();
        output.AtlasHeight = atlasData["height"]!.GetValue<int>();
        output.Distances = rawContent.AtlasData;

        JsonNode? metricsData = layoutDataRoot["metrics"];
        if (metricsData == null) {
            throw new Exception("No metrics data found.");
        }

        output.LineHeight = metricsData["lineHeight"]!.GetValue<float>();
        output.Ascender = metricsData["ascender"]!.GetValue<float>();
        output.Descender = metricsData["descender"]!.GetValue<float>();
        output.UnderlineY = metricsData["underlineY"]!.GetValue<float>();
        output.UnderlineThickness = metricsData["underlineThickness"]!.GetValue<float>();

        JsonNode? glyphsData = layoutDataRoot["glyphs"];
        if (glyphsData == null) {
            throw new Exception("No glyph data found.");
        }

        JsonArray glyphJsonArray = glyphsData.AsArray();
        GlyphContent[] glyphs = new GlyphContent[glyphJsonArray.Count];

        for (int i = 0; i < glyphJsonArray.Count; i++) {
            JsonNode? glyphData = glyphJsonArray[i];
            if (glyphData == null) {
                continue;
            }


        }

        output.Glyphs = glyphs;

        return output;
    }
}
