using System.Drawing;
using System.Numerics;

namespace image_processing_core;

public readonly struct RGB64
{
    public double R { get; }
    public double G {get; }
    public double B {get; }

    public RGB64(double r, double g, double b)
    {
        R = r;
        G = g;
        B = b;
    }
    
    #region RGB64 Operators

    public static RGB64 operator *(RGB64 rgb, int scalar)
    {
        return new RGB64(rgb.R * scalar, rgb.G * scalar, rgb.B * scalar);
    }
    
    public static RGB64 operator *(RGB64 rgb, float scalar)
    {
        return new RGB64(rgb.R * scalar, rgb.G * scalar, rgb.B * scalar);
    }
    
    public static RGB64 operator *(RGB64 rgb, double scalar)
    {
        return new RGB64(rgb.R * scalar, rgb.G * scalar, rgb.B * scalar);
    }
    
    public static RGB64 operator *(RGB64 rgb, RGB64 right)
    {
        return new RGB64(rgb.R * right.R, rgb.G * right.G, rgb.B * right.B);
    }
    
    public static RGB64 operator +(RGB64 left, int addend)
    {
        return new RGB64(left.R + addend, left.G + addend, left.B + addend);
    }
    
    public static RGB64 operator +(RGB64 left, RGB64 right)
    {
        return new RGB64(left.R + right.R, left.G + right.G, left.B + right.B);
    }
    
    public static RGB64 operator /(RGB64 left, RGB64 right)
    {
        return new RGB64(left.R / right.R, left.G / right.G, left.B / right.B);
    }
    
    public static RGB64 operator /(RGB64 left, int scalar)
    {
        return new RGB64(left.R / scalar, left.G / scalar, left.B / scalar);
    }
    
    public static RGB64 operator -(RGB64 left, RGB64 right)
    {
        return new RGB64(left.R - right.R, left.G - right.G, left.B - right.B);
    }
    
    public static bool operator >(RGB64 left, RGB64 right)
    {
        return left.Length() > right.Length();
    }
    
    public static bool operator <(RGB64 left, RGB64 right)
    {
        return left.Length() < right.Length();
    }
    

    #endregion

    #region RGB Operators

    public static RGB64 operator *(RGB64 rgb, RGB right)
    {
        return new RGB64(rgb.R * right.R, rgb.G * right.G, rgb.B * right.B);
    }

    public static RGB64 operator +(RGB64 left, RGB right)
    {
        return new RGB64(left.R + right.R, left.G + right.G, left.B + right.B);
    }
    
    public static RGB64 operator /(RGB64 left, RGB right)
    {
        return new RGB64(left.R / right.R, left.G / right.G, left.B / right.B);
    }

    public static RGB64 operator -(RGB64 left, RGB right)
    {
        return new RGB64(left.R - right.R, left.G - right.G, left.B - right.B);
    }
    
    public static bool operator >(RGB64 left, RGB right)
    {
        return left.Length() > right.Length();
    }
    
    public static bool operator <(RGB64 left, RGB right)
    {
        return left.Length() < right.Length();
    }

    #endregion
    
    public RGB64 AbsoluteValue()
    {
        return new RGB64(Math.Abs(R), Math.Abs(G),  Math.Abs(B));
    }

    public double Length()
    {
        return Math.Sqrt(R * R + G * G + B * B);
    }
    
    public RGB64 ChangeContrast(int factor)
    {
        double r = Math.Truncate(factor * (R - 128) + 128);
        double g = Math.Truncate(factor * (G - 128) + 128);
        double b = Math.Truncate(factor * (B - 128) + 128);
        return new RGB64(r, g, b);
    }
    
    public RGB64 Negative()
    {
        return new RGB64(255 - R, 255 - G, 255 - B);
    }

    public Color ToColor()
    {
        int r = Math.Clamp((int)R, 0, 255);
        int g = Math.Clamp((int)G, 0, 255);
        int b = Math.Clamp((int)B, 0, 255);
        
        return Color.FromArgb(255, r, g, b);
    }
    
    public static RGB64 ToRGB(Vector3 vector3)
    {
        return new RGB64(vector3.X, vector3.Y, vector3.Z);
    }

    public static RGB64 ToRGB(Color color)
    {
        return new RGB64(color.R, color.G, color.B);
    }
    
    public RGB64 Pow(double i)
    {
        var r = Math.Pow(R, i);
        var g = Math.Pow(G, i);
        var b = Math.Pow(B, i);
        return new RGB64(r, g, b);
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

    public static unsafe RGB64 ToRGB(byte* pixel)
    {
        return new RGB64(pixel[2], pixel[1], pixel[0]);
    }

    public double Mean()
    {
        return (R + G + B) / 3.0;
    }
    
    public override string ToString()
    {
        return base.ToString() + " R = " + R + ", G = " + G + ", B = " + B;
    }
}