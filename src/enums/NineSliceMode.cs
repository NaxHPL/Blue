namespace BlueFw;

/// <summary>
/// Defines how to draw a 9-sliced Sprite.
/// </summary>
public enum NineSliceMode {

    /// <summary>
    /// 9-slicing will now be used.
    /// </summary>
    None,

    /// <summary>
    /// The corners stay constant and the other sections scale.
    /// </summary>
    Scale,

    /// <summary>
    /// The corners stay constant and the other sections tile.
    /// </summary>
    Tile
}
