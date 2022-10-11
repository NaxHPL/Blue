using System.Text.Json.Nodes;

namespace BlueContentPipeline;
 
public struct GlyphContent {
    public int Codepoint;
    public float Advance;
    public Bounds PlaneBounds;
    public Bounds AtlasBounds;

    public GlyphContent(JsonNode glyphData) {
        Codepoint = glyphData["unicode"]!.GetValue<int>();
        Advance = glyphData["advance"]!.GetValue<float>();

        PlaneBounds = new Bounds();
        JsonNode? planeBoundsData = glyphData["planeBounds"];
        if (planeBoundsData != null) {
            PlaneBounds.Top = planeBoundsData["top"]!.GetValue<float>();
            PlaneBounds.Right = planeBoundsData["right"]!.GetValue<float>();
            PlaneBounds.Bottom = planeBoundsData["bottom"]!.GetValue<float>();
            PlaneBounds.Left = planeBoundsData["left"]!.GetValue<float>();
        }

        AtlasBounds = new Bounds();
        JsonNode? atlasBoundsData = glyphData["atlasBounds"];
        if (atlasBoundsData != null) {
            AtlasBounds = new Bounds();
        }
    }
}
