using Microsoft.Xna.Framework.Content.Pipeline;

namespace BlueContentPipeline;

[ContentImporter(
    /* Atlas image/layout formats */ ".bin", ".json",
    DisplayName = "SDF Font Importer - Blue",
    DefaultProcessor = "SDFFontProcessor"
)]
public class SDFFontImporter : ContentImporter<SDFFontRawContent> {

    public override SDFFontRawContent Import(string filePath, ContentImporterContext context) {
        string inputFilePathExt = Path.GetExtension(filePath);
        
        string atlasDataFilePath;
        string layoutDataFilePath;

        switch (inputFilePathExt) {
            case ".bin":
                atlasDataFilePath = filePath;
                layoutDataFilePath = Path.ChangeExtension(filePath, "json");
                break;
            case ".json":
                atlasDataFilePath = Path.ChangeExtension(filePath, "bin");
                layoutDataFilePath = filePath;
                break;
            default:
                throw new Exception($"The file extension {inputFilePathExt} is not supported.");
        }

        if (!File.Exists(atlasDataFilePath)) {
            throw new FileNotFoundException(null, atlasDataFilePath);
        }
        if (!File.Exists(layoutDataFilePath)) {
            throw new FileNotFoundException(null, layoutDataFilePath);
        }

        return new SDFFontRawContent {
            AtlasData = File.ReadAllBytes(atlasDataFilePath),
            LayoutDataJson = File.ReadAllText(layoutDataFilePath)
        };
    }
}
