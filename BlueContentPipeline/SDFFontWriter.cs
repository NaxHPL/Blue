using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BlueContentPipeline;

public class SDFFontWriter : ContentTypeWriter<SDFFontContent> {

    public override string GetRuntimeReader(TargetPlatform targetPlatform) {
        return "BlueFw.SDFFontReader";
    }

    protected override void Write(ContentWriter output, SDFFontContent fontContent) {
        
    }
}
