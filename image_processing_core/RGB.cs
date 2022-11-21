using System.Drawing;
using System.Numerics;

namespace image_processing_core;

public struct RGB
{
    public short R { get; private set; }
    public short G {get; private set; }
    public short B {get; private set; }

    public RGB(long r, long g, long b)
    {
        R = (short)r;
        G = (short)g;
        B = (short)b;
    }
    
    public RGB(int r, int g, int b)
    {
        R = (short)r;
        G = (short)g;
        B = (short)b;
    }
    
    public RGB(short r, short g, short b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static RGB Zero()
    {
        return new RGB(0, 0, 0);
    }

    #region RGB Operators

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
        return left.Length() > right.Length();
    }
    
    public static bool operator <(RGB left, RGB right)
    {
        return left.Length() < right.Length();
    }
    
    #endregion
    
    #region RGB64 Operators

    public static RGB operator *(RGB rgb, RGB64 right)
    {
        return new RGB((int)(rgb.R * right.R), (int)(rgb.G * right.G), (int)(rgb.B * right.B));
    }

    public static RGB operator +(RGB left, RGB64 right)
    {
        return new RGB((int)(left.R + right.R), (int)(left.G + right.G), (int)(left.B + right.B));
    }
    
    public static RGB operator /(RGB left, RGB64 right)
    {
        return new RGB((int)(left.R / right.R), (int)(left.G / right.G), (int)(left.B / right.B));
    }
    
    public static RGB operator -(RGB left, RGB64 right)
    {
        return new RGB((int)(left.R - right.R), (int)(left.G - right.G), (int)(left.B - right.B));
    }
    
    public static bool operator >(RGB left, RGB64 right)
    {
        return left.Length() > right.Length();
    }
    
    public static bool operator <(RGB left, RGB64 right)
    {
        return left.Length() < right.Length();
    }

    #endregion

    public RGB AbsoluteValue()
    {
        return new RGB(Math.Abs(R), Math.Abs(G),  Math.Abs(B));
    }

    public double Length()
    {
        return Math.Sqrt(R ^ 2 + G ^ 2 + B ^ 2);
    }

    public bool ContainsZero()
    {
        return R == 0 || B == 0 || G == 0;
    }
    
    public RGB ChangeContrast(float factor)
    {
        var r = (int)(factor * (R - 128) + 128);
        var g = (int)(factor * (G - 128) + 128);
        var b = (int)(factor * (B - 128) + 128);
        return new RGB(r, g, b);
    }
    
    public RGB Negative()
    {
        return new RGB(255 - R, 255 - G, 255 - B);
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

    public static RGB ToRGB(Vector3 vector3)
    {
        return new RGB((short)vector3.X, (short)vector3.Y, (short)vector3.Z);
    }

    public static RGB ToRGB(Color color)
    {
        return new RGB(color.R, color.G, color.B);
    }

    public unsafe void SaveToPixel(byte* pixel)
    {
        var r = (byte)Math.Clamp((int)R, 0, 255);
        var g = (byte)Math.Clamp((int)G, 0, 255);
        var b = (byte)Math.Clamp((int)B, 0, 255);

        pixel[2] = r;
        pixel[1] = g;
        pixel[0] = b;
    }

    public static unsafe RGB ToRGB(byte* pixel)
    {
        return new RGB(pixel[2], pixel[1], pixel[0]);
    }

    public RGB SetOneWhenZero()
    {
        if (R == 0) R = 1;
        if (G == 0) G = 1;
        if (B == 0) B = 1;
        return this;
    }

    public override string ToString()
    {
        return $"{base.ToString()} R = {R}, G = {G}, B = {B}";
    }
}