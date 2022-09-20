using Microsoft.Xna.Framework.Content.Pipeline;

namespace BlueContentPipeline;

[ContentImporter(".ttf", ".otf", DisplayName = "SDF Font Importer - Blue", DefaultProcessor = "SDFFontProcessor")]
public class SDFFontImporter : ContentImporter<string> {

    public override string Import(string filePath, ContentImporterContext context) {
        return filePath;
    }
}
