using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BlueContentPipeline;

public class SDFFontWriter : ContentTypeWriter<SDFFontContent> {

    protected override void Write(ContentWriter output, SDFFontContent fontContent) {
        output.Write(fontContent.AtlasWidth);
        
        //output.Write(fontContent.AtlasWidth);
        //output.Write(fontContent.AtlasHeight);
        //foreach (byte color in fontContent.AtlasPixelData) {
        //    output.Write(color);
        //}
        //output.Write(fontContent.LayoutDataJson);
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform) {
        return "BlueFw.Content.SDFFontReader";
    }
}
