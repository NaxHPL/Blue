using System.Text.Json.Nodes;

namespace BlueContentPipeline;

public struct KerningPair {

    public int Codepoint1;
    public int Codepoint2;
    public float Advance;

    public KerningPair(JsonNode? jsonData) {
        if (jsonData == null) {
            this = default;
            return;
        }

        Codepoint1 = jsonData["unicode1"]!.GetValue<int>();
        Codepoint2 = jsonData["unicode2"]!.GetValue<int>();
        Advance = jsonData["advance"]!.GetValue<float>();
    }
}
