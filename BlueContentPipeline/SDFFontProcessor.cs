using Microsoft.Xna.Framework.Content.Pipeline;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace BlueContentPipeline;

[ContentProcessor(DisplayName = "SDF Font Processor - Blue")]
public class SDFFontProcessor : ContentProcessor<string, SDFFontContent> {

    [DefaultValue("[0x20, 0x7E]")]
    [DisplayName("Character Set")]
    [Description(
        "The characters can be denoted in the following ways:"                                          + "\n" +
        "  - Single character: 'A' (UTF-8 encoded), 65 (decimal Unicode), 0x41 (hexadecimal Unicode)"   + "\n" +
        "  - Range of characters: ['A', 'Z'], [65, 90], [0x41, 0x5a]"                                   + "\n" +
        "  - String of characters: \"ABCDEFGHIJKLMNOPQRSTUVWXYZ\" (UTF-8 encoded)"                      + "\n" +
        "The entries should be separated by commas or whitespace. In between quotation marks, backslash is used as the escape character (e.g. '\\'', '\\\\', \"!\\\"#\"). The order in which characters appear is not taken into consideration."
    )]
    public string CharacterSet { get; set; } = "[0x20, 0x7E]";

    [DefaultValue(false)]
    [DisplayName("Use Fixed Atlas Dimensions")]
    [Description("If true, the fixed dimensions will be used; otherwise, the minimum possible atlas dimensions will be selected based on the Automatic Dimensions Contraint parameter.")]
    public bool UseFixedAtlasDimensions { get; set; } = false;

    [DefaultValue(256)]
    [DisplayName("Fixed Atlas Width")]
    [Description("The atlas' fixed width (only used if Use Fixed Atlas Dimensions is set to true)")]
    public int FixedAtlasWidth { get; set; } = 256;

    [DefaultValue(256)]
    [DisplayName("Fixed Atlas Height")]
    [Description("The atlas' fixed height (only used if Use Fixed Atlas Dimensions is set to true)")]
    public int FixedAtlasHeight { get; set; } = 256;

    [DefaultValue(typeof(AutoDimensionsContraint), "SquareDivisibleByFour")]
    [DisplayName("Automatic Dimensions Constraint")]
    [Description("Sets the constraint to use when automatically sizing the atlas (only used if Use Fixed Atlas Dimensions is set to false)")]
    public AutoDimensionsContraint AutoDimensionsContraint { get; set; } = AutoDimensionsContraint.SquareDivisibleByFour;

    [DefaultValue(36)]
    [DisplayName("Glyph Size")]
    [Description("The size of the glyphs in the atlas in pixels per EM")]
    public int GlyphSize { get; set; } = 36;

    [DefaultValue(8)]
    [DisplayName("Distance Field Range Pixels")]
    [Description("The range of the distance field in pixels")]
    public int DistanceFieldRangePx { get; set; } = 8;

    public override SDFFontContent Process(string fontFilePath, ContentProcessorContext context) {
        string fontName = Path.GetFileNameWithoutExtension(fontFilePath);
        string id = "00000000";//Guid.NewGuid().ToString()[..8];

        string charSetFilePath = $"{context.IntermediateDirectory}{fontName}_charset_{id}.txt";
        string layoutDataFilePath = $"{context.IntermediateDirectory}{fontName}_layoutdata_{id}.json";
        string atlasImageFilePath = $"{context.IntermediateDirectory}{fontName}_atlas_{id}.bin";

        using (StreamWriter streamWriter = File.CreateText(charSetFilePath)) {
            streamWriter.Write(CharacterSet);
        }

        // Use msdf-atlas-gen to make atlas and layout data

        string layoutDataJson = File.ReadAllText(layoutDataFilePath);

        JsonNode layoutDataNode = JsonNode.Parse(layoutDataJson)!;
        int atlasWidth = layoutDataNode["atlas"]!["width"]!.GetValue<int>();
        int atlasHeight = layoutDataNode["atlas"]!["height"]!.GetValue<int>();

        byte[] atlasPixelData;
        using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(atlasImageFilePath))) {
            atlasPixelData = binaryReader.ReadBytes(atlasWidth * atlasHeight);
        }

        File.Delete(charSetFilePath);
        //File.Delete(layoutDataFilePath);
        //File.Delete(atlasImageFilePath);

        return new SDFFontContent() {
            AtlasWidth = atlasWidth,
            AtlasHeight = atlasHeight,
            AtlasPixelData = atlasPixelData,
            LayoutDataJson = layoutDataJson
        };
    }
}
