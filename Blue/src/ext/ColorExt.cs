using Microsoft.Xna.Framework;

namespace BlueFw;

public static class ColorExt {

    /// <summary>
    /// Returns a color which is a grayscale representation of this one.
    /// </summary>
    public static Color ToGrayscale(this Color color) {
        // Rec. 709 luma component. https://en.wikipedia.org/wiki/Grayscale#Luma_coding_in_video_systems
        byte gray = (byte)(0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B);
        color.R = color.G = color.B = gray;
        return color;
    }

    /// <summary>
    /// Returns a new color which is the result of subtracting the RGBA values of <paramref name="color"/> from the RGBA values of this color.
    /// </summary>
    public static Color Subtract(this Color thisColor, Color color) {
        return new Color(
            thisColor.R - color.R,
            thisColor.G - color.G,
            thisColor.B - color.B,
            thisColor.A - color.A
        );
    }

    /// <summary>
    /// Returns a new color which is the result of adding the RGBA values of <paramref name="color"/> and this color.
    /// </summary>
    public static Color Add(this Color thisColor, Color color) {
        return new Color(
            thisColor.R + color.R,
            thisColor.G + color.G,
            thisColor.B + color.B,
            thisColor.A + color.A
        );
    }
}
