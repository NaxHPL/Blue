using Microsoft.Xna.Framework.Content.Pipeline;
using System.ComponentModel;
using System;
using FreeImageAPI;
using System.Drawing;

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

    [DefaultValue(6)]
    [DisplayName("Distance Field Range Pixels")]
    [Description("The range of the distance field in pixels")]
    public int DistanceFieldRangePx { get; set; } = 6;

    public override SDFFontContent Process(string fontFilePath, ContentProcessorContext context) {
        string fontName = Path.GetFileNameWithoutExtension(fontFilePath);
        string id = "test";//Guid.NewGuid().ToString()[..8];

        string charSetFilePath = $"{context.IntermediateDirectory}{fontName}_charset_{id}.txt";
        string layoutDataFilePath = $"{context.IntermediateDirectory}{fontName}_layoutdata_{id}.json";
        string atlasImageFilePath = $"{context.IntermediateDirectory}{fontName}_atlas_{id}.png";

        using (StreamWriter streamWriter = File.CreateText(charSetFilePath)) {
            streamWriter.Write(CharacterSet);
        }

        // Use msdf-atlas-gen to make atlas and layout data

        // load atlas image and fill SDFFontContent.PixelData
        FIBITMAP fiBitmap = FreeImage.Load(FREE_IMAGE_FORMAT.FIF_PNG, atlasImageFilePath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);

        uint atlasWidth = FreeImage.GetWidth(fiBitmap);
        uint atlasHeight = FreeImage.GetHeight(fiBitmap);
        Color[] atlasPixelData = new Color[atlasWidth * atlasHeight];

        for (uint y = 0; y < atlasHeight; y++) {
            for (uint x = 0; x < atlasWidth; x++) {
                FreeImage.GetPixelColor(fiBitmap, x, y, out RGBQUAD rgbQuad);
                atlasPixelData[y * atlasWidth + x] = rgbQuad.Color;
            }
        }

        FreeImage.Unload(fiBitmap);

        // read layout data and fill SDFFontContent.LayoutDataJson
        string layoutDataJson = File.ReadAllText(layoutDataFilePath);

        File.Delete(charSetFilePath);
        File.Delete(layoutDataFilePath);
        File.Delete(atlasImageFilePath);

        return new SDFFontContent() {
            AtlasWidth = atlasWidth,
            AtlasHeight = atlasHeight,
            AtlasPixelData = atlasPixelData,
            LayoutDataJson = layoutDataJson
        };
    }
}
