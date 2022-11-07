using System.Numerics;

namespace image_processing_core;

public static class MathHelper
{

    public static float Lerp(float v1, float v2, float t)
    {
        return v1 + (v2 - v1) * t;
    }

    public static RGB LerpRGB(RGB rgb1, RGB rgb2, float t)
    {
        float r1 = rgb1.R / 255.0f; float r2 = rgb2.R / 255.0f;
        float g1 = rgb1.G / 255.0f; float g2 = rgb2.G / 255.0f;
        float b1 = rgb1.B / 255.0f; float b2 = rgb2.B / 255.0f;

        r1 = Lerp(r1, r2, t);
        g1 = Lerp(g1, g2, t);
        b1 = Lerp(b1, b2, t);

        return new RGB((short)(r1 * 255), (short)(b1 * 255), (short)(g1 * 255));
    }

    public static RGB BilinearInterpolation(RGB p00, RGB p01, RGB p10, RGB p11, Vector2 pixelPos)
    {
        float t = pixelPos.X;
        RGB rgb1 = LerpRGB(p00, p01, t);
        RGB rgb2 = LerpRGB(p10, p11, t);
        
        t = pixelPos.Y;
        return LerpRGB(rgb1, rgb2, t);
    }
    
    public static float SmoothStep(float t)
    {
        float v1 = t * t;
        float v2 = 1.0f - (1.0f - t) * (1.0f - t);
        return Lerp(v1, v2, t);
    }

    public static float SmootherStep(float v1, float v2, float x) {

        x = Math.Clamp((x - v1) / (v2 - v1), 0.0f, 1.0f);

        return x * x * x * (x * (x * 6 - 15) + 10);
    }

    public static float PerlinInterpolation(float v1, float v2, float t)
    {
        return v1 + (v2 - v1) * SmootherStep(v1, v2, t);
    }

    public static RGB PerlinInterpolationRGB(RGB rgb1, RGB rgb2, float t)
    {
        float r1 = rgb1.R / 255.0f; float r2 = rgb2.R / 255.0f;
        float g1 = rgb1.G / 255.0f; float g2 = rgb2.G / 255.0f;
        float b1 = rgb1.B / 255.0f; float b2 = rgb2.B / 255.0f;

        r1 = PerlinInterpolation(r1, r2, t);
        g1 = PerlinInterpolation(g1, g2, t);
        b1 = PerlinInterpolation(b1, b2, t);

        return new RGB((short)(r1 * 255), (short)(b1 * 255), (short)(g1 * 255));
    }

    public static RGB PerlinInterpPixels(RGB p00, RGB p01, RGB p10, RGB p11, Vector2 pixelPos)
    {
        float t = pixelPos.X;
        RGB rgb1 = PerlinInterpolationRGB(p00, p01, t);
        RGB rgb2 = PerlinInterpolationRGB(p10, p11, t);
        
        t = pixelPos.Y;
        return PerlinInterpolationRGB(rgb1, rgb2, t);
    }

    public static RGB MedianRGB(RGB[] rgbs)
    {
        int length = rgbs.Length;
        
        var r = new int[length];
        var g = new int[length];
        var b = new int[length];

        for (var i = 0; i < length; i++)
        {
            r[i] = rgbs[i].R;
            g[i] = rgbs[i].G;
            b[i] = rgbs[i].B;
        }

        Array.Sort(r);
        Array.Sort(g);
        Array.Sort(b);

        int mid = length / 2;
        if(length % 2 == 0) return new RGB(r[mid], g[mid], b[mid]);
        
        return new RGB(
            (r[mid] + r[mid - 1])/2,
            (g[mid] + g[mid - 1])/2,
            (b[mid] + b[mid - 1])/2);
    }
}