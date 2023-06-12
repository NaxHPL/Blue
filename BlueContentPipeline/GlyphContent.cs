using System.Text.Json.Nodes;

namespace BlueContentPipeline;
 
public struct GlyphContent {

    public int Codepoint;
    public float Advance;
    public Bounds PlaneBounds;
    public Bounds AtlasBounds;

    public GlyphContent(JsonNode? glyphData) {
        if (glyphData == null) {
            this = default;
            return;
        }

        Codepoint = glyphData["unicode"]!.GetValue<int>();
        Advance = glyphData["advance"]!.GetValue<float>();
        PlaneBounds = new Bounds(glyphData["planeBounds"]);
        AtlasBounds = new Bounds(glyphData["atlasBounds"]);
    }
}
