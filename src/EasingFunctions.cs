using Microsoft.Xna.Framework;
using System;

namespace Blue;

public delegate float EasingFunc(float x);
public delegate float EasingFunc<T>(float x, T param);
public delegate float EasingFunc<T1, T2>(float x, T1 param1, T2 param2);

public static class EasingFunctions {

    public static readonly EasingFunc Linear = (x) => x;

    public static readonly EasingFunc SineIn = (x) => 1f - MathF.Cos(x * MathHelper.PiOver2);
    public static readonly EasingFunc SineOut = (x) => MathF.Sin(x * MathHelper.PiOver2);
    public static readonly EasingFunc SineInOut = (x) => -(MathF.Cos(x * MathHelper.Pi) - 1f) * 0.5f;

    public static readonly EasingFunc QuadIn = (x) => x * x;
    public static readonly EasingFunc QuadOut = (x) => 1f - MathF.Pow(1f - x, 2f);
    public static readonly EasingFunc QuadInOut = (x) => {
        return x < 0.5f ?
            2f * x * x :
            1f - MathF.Pow(-2f * x + 2f, 2f) * 0.5f;
    };

    public static readonly EasingFunc CubicIn = (x) => x * x * x;
    public static readonly EasingFunc CubicOut = (x) => 1f - MathF.Pow(1f - x, 3f);
    public static readonly EasingFunc CubicInOut = (x) => {
        return x < 0.5f ?
            4f * x * x * x :
            1f - MathF.Pow(-2f * x + 2f, 3f) * 0.5f;
    };

    public static readonly EasingFunc QuartIn = (x) => x * x * x * x;
    public static readonly EasingFunc QuartOut = (x) => 1f - MathF.Pow(1f - x, 4f);
    public static readonly EasingFunc QuartInOut = (x) => {
        return x < 0.5f ?
            8f * x * x * x * x :
            1f - MathF.Pow(-2f * x + 2f, 4f) * 0.5f;
    };

    public static readonly EasingFunc QuintIn = (x) => x * x * x * x * x;
    public static readonly EasingFunc QuintOut = (x) => 1f - MathF.Pow(1f - x, 5f);
    public static readonly EasingFunc QuintInOut = (x) => {
        return x < 0.5f ?
            16f * x * x * x * x * x :
            1f - MathF.Pow(-2f * x + 2f, 5f) * 0.5f;
    };

    public static readonly EasingFunc ExpoIn = (x) => x == 0f ? 0f : MathF.Pow(2f, 10f * x - 10f);
    public static readonly EasingFunc ExpoOut = (x) => x == 1f ? 1f : 1f - MathF.Pow(2f, -10f * x);
    public static readonly EasingFunc ExpoInOut = (x) => {
        if (x == 0f || x == 1f) {
            return x;
        }

        return x < 0.5f ?
            MathF.Pow(2f, 20f * x - 10f) * 0.5f :
            (2f - MathF.Pow(2f, -20f * x + 10f)) * 0.5f;
    };

    public static readonly EasingFunc CircIn = (x) => 1f - MathF.Sqrt(1f - x * x);
    public static readonly EasingFunc CircOut = (x) => MathF.Sqrt(1f - MathF.Pow(x - 1f, 2f));
    public static readonly EasingFunc CircInOut = (x) => {
        return x < 0.5f ?
            (1f - MathF.Sqrt(1f - 4f * x * x)) * 0.5f :
            (MathF.Sqrt(1f - MathF.Pow(-2f * x + 2f, 2f)) + 1f) * 0.5f;
    };

    public static readonly EasingFunc BackIn = (x) => BackInExt(x, 1.7016f);
    public static readonly EasingFunc<float> BackInExt = (x, amplitude) => (amplitude + 1f) * x * x * x - amplitude * x * x;
    public static readonly EasingFunc BackOut = (x) => BackOutExt(x, 1.7016f);
    public static readonly EasingFunc<float> BackOutExt = (x, amplitude) => 1f - BackInExt(1f - x, amplitude);
    public static readonly EasingFunc BackInOut = (x) => BackInOutExt(x, 1.7016f);
    public static readonly EasingFunc<float> BackInOutExt = (x, amplitude) => {
        return x < 0.5f ?
            0.5f * BackInExt(2f * x, amplitude) :
            1f - 0.5f * BackInExt(2f - 2f * x, amplitude);
    };

    public static readonly EasingFunc ElasticIn = (x) => ElasticInExt(x, 2, 5f);
    public static readonly EasingFunc<int, float> ElasticInExt = (x, oscillations, stiffness) => {
        float e = (MathF.Exp(x * stiffness) - 1f) / (MathF.Exp(stiffness) - 1f);
        return e * MathF.Sin(x * (MathHelper.PiOver2 + 2f * MathHelper.Pi * oscillations));

    };
    public static readonly EasingFunc ElasticOut = (x) => ElasticOutExt(x, 2, 5f);
    public static readonly EasingFunc<int, float> ElasticOutExt = (x, oscillations, stiffness) => 1f - ElasticInExt(1f - x, oscillations, stiffness);
    public static readonly EasingFunc ElasticInOut = (x) => ElasticInOutExt(x, 2, 5f);
    public static readonly EasingFunc<int, float> ElasticInOutExt = (x, oscillations, stiffness) => {
        return x < 0.5f ?
            0.5f * ElasticInExt(2f * x, oscillations, stiffness) :
            1f - 0.5f * ElasticInExt(2f - 2f * x, oscillations, stiffness);
    };

    public static readonly EasingFunc BounceIn = (x) => 1f - BounceOut(1f - x);
    public static readonly EasingFunc BounceOut = (x) => {
        const float n1 = 7.5625f;
        const float d1 = 2.75f;

        if (x < 1 / d1) {
            return n1 * x * x;
        }
        else if (x < 2 / d1) {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        }
        else if (x < 2.5 / d1) {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        }

        return n1 * (x -= 2.625f / d1) * x + 0.984375f;
    };
    public static readonly EasingFunc BounceInOut = (x) => {
        return x < 0.5f ?
            (1f - BounceOut(1f - 2f * x)) * 0.5f :
            (1f + BounceOut(2f * x - 1f)) * 0.5f;
    };
}
