using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing;
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
                ushort i = 0;
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        var rbg = RGB.ToRGB(pixel);
                        if(rbg.ContainsZero()) continue;
                        var value = rbg.ToVector3();
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
                RGB64 sum = new RGB64(0,0,0);
                ushort i = 0;
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
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
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
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
    
    public static unsafe void MedianFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;
            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = row + x * bpp;
                var rgbs = new List<RGB>();
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* rRow = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
                        byte* rPixel = rRow + rx * bpp;
                        rgbs.Add(RGB.ToRGB(rPixel));
                    }
                }
                
                MathHelper.MedianRGB(rgbs.ToArray()).SaveToPixel(pixel);
            }
        }
    }

    public static unsafe void GeometricMeanFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                var result = new RGB64(1, 1, 1);
                ushort i = 0;
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        var color = RGB.ToRGB(pixel);
                        result *= color.SetOneWhenZero();
                        i++;
                    }
                }

                result = result.Pow(1.0 / i);
                result.SaveToPixel(pt + y * data.Stride + x * bpp);
            }
        }
    }
    
    public static unsafe void MaxFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                var max = new RGB(0,   0,   0);
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        if(RGB.ToRGB(pixel) > max) max = RGB.ToRGB(pixel);
                    }
                }
                
                max.SaveToPixel(pt + y * data.Stride + x * bpp);
            }
        }
    }
    
    public static unsafe void MinFilter(BitmapData data, Rectangle rect)
    {
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                var min = new RGB(255,   255,   255);
                for (int ry = y - offsetY; ry < rect.Height + y - offsetY; ry++)
                {
                    if(ry < 0 || ry >= data.Height) continue;
                    byte* row = pt + ry * data.Stride;
                    for (int rx = x - offsetX; rx < rect.Width + x - offsetX; rx++)
                    {
                        if(rx < 0 || rx >= data.Width) continue;
                        byte* pixel = row + rx * bpp;
                        var p = RGB.ToRGB(pixel);
                        if(p < min) min = p;
                    }
                }
                
                min.SaveToPixel(pt + y * data.Stride + x * bpp);
            }
        }
    }

    public static unsafe Bitmap MinimumFilter(Bitmap bitmap, Rectangle rect)
    {
        var newBitmap = new Bitmap(bitmap);
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);
        
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (int y = 0; y < data.Height; y++)
        {
            for (int x = 0; x < data.Width; x++)
            {
                var min = new RGB(255, 255, 255);

                for (int dy = -offsetY; dy <= offsetY; dy++)
                {
                    if(dy + y < 0 || dy + y >= data.Height) continue;
                    byte* row = pt + (dy + y) * data.Stride;
                    for (int dx = -offsetX; dx <= offsetX; dx++)
                    {
                        if(dx + x < 0 || dx + x >= data.Width) continue;
                        byte* pixel = row + (x + dx) * bpp;
                        var p = RGB.ToRGB(pixel);
                        if (p < min) min = p;
                    }
                }
                
                min.SaveToPixel((byte*)newData.Scan0 + y * newData.Stride + x * newData.Stride / newData.Width);
            }
        }

        newBitmap.UnlockBits(newData);
        return newBitmap;
    }
    
    public static unsafe Bitmap MaximumFilter(Bitmap bitmap, Rectangle rect)
    {
        var newBitmap = new Bitmap(bitmap);
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);
        
        int offsetX = rect.Width / 2;
        int offsetY = rect.Height / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (int y = 0; y < data.Height; y++)
        {
            for (int x = 0; x < data.Width; x++)
            {
                var max = new RGB(0, 0, 0);

                for (int dy = -offsetY; dy <= offsetY; dy++)
                {
                    if(dy + y < 0 || dy + y >= data.Height) continue;
                    byte* row = pt + (dy + y) * data.Stride;
                    for (int dx = -offsetX; dx <= offsetX; dx++)
                    {
                        if(dx + x < 0 || dx + x >= data.Width) continue;
                        byte* pixel = row + (x + dx) * bpp;
                        var p = RGB.ToRGB(pixel);
                        if (p > max) max = p;
                    }
                }
                
                max.SaveToPixel((byte*)newData.Scan0 + y * newData.Stride + x * newData.Stride / newData.Width);
            }
        }

        newBitmap.UnlockBits(newData);
        return newBitmap;
    }
}