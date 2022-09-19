using Microsoft.Xna.Framework.Graphics;
using System;

namespace BlueFw;

public class Material {

    /// <summary>
    /// The default material.<br/>
    /// Renderables with a <see langword="null"/> material will use this.
    /// </summary>
    public static Material Default {
        get => defaultMat;
        set => defaultMat = value ?? throw new ArgumentNullException(nameof(value));
    }
    static Material defaultMat = new Material();

    /// <summary>
    /// The material's blend state.
    /// </summary>
    public BlendState BlendState = BlendState.NonPremultiplied;

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

    public Material() { }

    public Material(Material cloneMaterial) {
        BlendState = cloneMaterial.BlendState;
        SamplerState = cloneMaterial.SamplerState;
        DepthStencilState = cloneMaterial.DepthStencilState;
        RasterizerState = cloneMaterial.RasterizerState;
        Effect = cloneMaterial.Effect;
    }

    public Material Clone() {
        return new Material() {
            BlendState = BlendState,
            SamplerState = SamplerState,
            DepthStencilState = DepthStencilState,
            RasterizerState = RasterizerState,
            Effect = Effect
        };
    }
}
