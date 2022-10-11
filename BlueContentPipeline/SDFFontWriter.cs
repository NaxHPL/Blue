using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BlueContentPipeline;

[ContentTypeWriter]
public class SDFFontWriter : ContentTypeWriter<SDFFontContent> {

    protected override void Write(ContentWriter writer, SDFFontContent fontContent) {
        
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform) {
        return "BlueFw.Content.SDFFontReader, Blue";
    }

    public override string GetRuntimeType(TargetPlatform targetPlatform) {
        return "BlueFw.Content.SDFFont, Blue";
    }
}
