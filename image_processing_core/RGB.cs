using System.Drawing;
using System.Numerics;

namespace image_processing_core;

public struct RGB
{
    private int R { get; }
    private int G {get; }
    private int B {get; }

    public RGB(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static RGB Zero()
    {
        return new RGB(0, 0, 0);
    }
    public static RGB operator *(RGB rgb, int scalar)
    {
        return new RGB(rgb.R * scalar, rgb.G * scalar, rgb.B * scalar);
    }
    
    public static RGB operator *(RGB rgb, RGB right)
    {
        return new RGB(rgb.R * right.R, rgb.G * right.G, rgb.B * right.B);
    }
    
    public static RGB operator +(RGB left, int addend)
    {
        return new RGB(left.R + addend, left.G + addend, left.B + addend);
    }
    
    public static RGB operator +(RGB left, RGB right)
    {
        return new RGB(left.R + right.R, left.G + right.G, left.B + right.B);
    }
    
    public static RGB operator /(RGB left, RGB right)
    {
        return new RGB(left.R / right.R, left.G / right.G, left.B / right.B);
    }
    
    public static RGB operator /(RGB left, int scalar)
    {
        return new RGB(left.R / scalar, left.G / scalar, left.B / scalar);
    }
    
    public static RGB operator -(RGB left, RGB right)
    {
        return new RGB(left.R - right.R, left.G - right.G, left.B - right.B);
    }
    
    public static bool operator >(RGB left, RGB right)
    {
        return left.VectorLength() > right.VectorLength();
    }
    
    public static bool operator <(RGB left, RGB right)
    {
        return left.VectorLength() < right.VectorLength();
    }

    public double VectorLength()
    {
        return Math.Sqrt(R ^ 2 + G ^ 2 + B ^ 2);
    }
    
    public RGB ChangeContrast(int factor)
    {
        var r = (int)Math.Truncate((double)(factor * (R - 128) + 128));
        var g = (int)Math.Truncate((double)(factor * (G - 128) + 128));
        var b = (int)Math.Truncate((double)(factor * (B - 128) + 128));
        return new RGB(r, g, b);
    }
    
    public RGB Negative()
    {
        return new RGB(255 - R, 255 - G, 255 - B);
    }

    public Color ToColor()
    {
        int r = Math.Clamp(R, 0, 255);
        int g = Math.Clamp(G, 0, 255);
        int b = Math.Clamp(B, 0, 255);
        
        return Color.FromArgb(255, r, g, b);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(R, G, B);
    }

    public static RGB ToRGB(Vector3 vector3)
    {
        return new RGB((int)vector3.X, (int)vector3.Y, (int)vector3.Z);
    }

    public static RGB ToRGB(Color color)
    {
        return new RGB(color.R, color.G, color.B);
    }

    public unsafe void SaveToPixel(byte* pixel)
    {
        var r = (byte)Math.Clamp(R, 0, 255);
        var g = (byte)Math.Clamp(G, 0, 255);
        var b = (byte)Math.Clamp(B, 0, 255);

        pixel[2] = r;
        pixel[1] = g;
        pixel[0] = b;
    }

    public static unsafe RGB ToRGB(byte* pixel)
    {
        return new RGB(pixel[2], pixel[1], pixel[0]);
    }
}