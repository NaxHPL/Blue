using Microsoft.Xna.Framework.Graphics;

namespace BlueFw;

public class Material {

    /// <summary>
    /// The default material.
    /// </summary>
    /// <remarks>
    /// Renderables with a <see langword="null"/> material will use this.
    /// </remarks>
    public static Material Default = new Material();

    /// <summary>
    /// The material's blend state.
    /// </summary>
    public BlendState BlendState = BlendState.AlphaBlend;

    /// <summary>
    /// The material's sampler state.
    /// </summary>
    public SamplerState SamplerState = SamplerState.LinearClamp;

    /// <summary>
    /// The material's depth/stencil state.
    /// </summary>
    public DepthStencilState DepthStencilState = DepthStencilState.None;

    /// <summary>
    /// The material's resterizer state.
    /// </summary>
    public RasterizerState RasterizerState = RasterizerState.CullCounterClockwise;

    /// <summary>
    /// The material's effect.
    /// </summary>
    public Effect Effect;
}
