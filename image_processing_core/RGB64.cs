using System.Drawing;
using System.Numerics;

namespace image_processing_core;

public readonly struct RGB64
{
    public long R { get; }
    public long G {get; }
    public long B {get; }

    public RGB64(long r, long g, long b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static RGB64 Zero()
    {
        return new RGB64(0, 0, 0);
    }

    #region RGB64 Operators

    public static RGB64 operator *(RGB64 rgb, int scalar)
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
        return Math.Sqrt(R ^ 2 + G ^ 2 + B ^ 2);
    }
    
    public RGB64 ChangeContrast(int factor)
    {
        var r = (int)Math.Truncate((double)(factor * (R - 128) + 128));
        var g = (int)Math.Truncate((double)(factor * (G - 128) + 128));
        var b = (int)Math.Truncate((double)(factor * (B - 128) + 128));
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

    public Vector3 ToVector3()
    {
        return new Vector3(R, G, B);
    }

    public static RGB64 ToRGB(Vector3 vector3)
    {
        return new RGB64((int)vector3.X, (int)vector3.Y, (int)vector3.Z);
    }

    public static RGB64 ToRGB(Color color)
    {
        return new RGB64(color.R, color.G, color.B);
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

    public override string ToString()
    {
        return base.ToString() + " R = " + R + ", G = " + G + ", B = " + B;
    }
}