using System.Text.Json.Nodes;

namespace BlueContentPipeline;

public struct Bounds {

    public float Top;
    public float Right;
    public float Bottom;
    public float Left;

    public Bounds(JsonNode? jsonData) {
        if (jsonData == null) {
            this = default;
            return;
        }

        Top = jsonData["top"]!.GetValue<float>();
        Right = jsonData["right"]!.GetValue<float>();
        Bottom = jsonData["bottom"]!.GetValue<float>();
        Left = jsonData["left"]!.GetValue<float>();
    }
}
