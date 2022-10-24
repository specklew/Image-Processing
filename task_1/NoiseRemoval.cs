using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing_core;

namespace task_1;

public static class NoiseRemoval
{
    public static unsafe void HarmonicFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                Vector3 meanValues = Vector3.Zero;
                byte i = 0;
                for (var ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry > data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (var rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx > data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        Vector3 value = RGB.ToRGB(pixel).ToVector3();
                        meanValues += Vector3.One / value;
                        i++;
                    }
                }

                Vector3 sum = i * Vector3.One / meanValues;
                RGB.ToRGB(sum).SaveToPixel(pt + y * data.Stride + x * bpp);
            }
        }
    }
    
    public static unsafe void ArithmeticMeanFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                RGB sum = RGB.Zero();
                int i = 0;
                for (var ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry > data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (var rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx > data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        sum += RGB.ToRGB(pixel);
                        i++;
                    }
                }

                sum /= i;
                sum.SaveToPixel(pt + y * data.Stride + x * bpp);
            }
        }
    }
    
    public static unsafe void MidpointFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                var min = new RGB(255, 255, 255);
                var max = new RGB(0,   0,   0);
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry > data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx > data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        if(RGB.ToRGB(pixel) > max) max = RGB.ToRGB(pixel); 
                        if(RGB.ToRGB(pixel) < min) min = RGB.ToRGB(pixel); 
                    }
                }

                RGB sum = min + max / 2;
                sum.SaveToPixel(pt + y * data.Stride + x * bpp);
            }
        }
    }
}