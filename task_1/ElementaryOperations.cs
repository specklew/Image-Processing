using System.Drawing.Imaging;
using image_processing_core;
using JetBrains.Profiler.Api;

namespace task_1;

public static class ElementaryOperations
{
    public static unsafe void ModifyBrightness(BitmapData data, int brightness)
    {

        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;

            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = row + x * bpp;

                var rgb = RGB.ToRGB(pixel);
                rgb += brightness;
                rgb.SaveToPixel(pixel);
            }
        }
    }
    
    public static unsafe void ModifyContrast(BitmapData data, int contrast)
    {
        float factor = 259 * (contrast + 255);
        factor /= 255 * (259 - contrast);   
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;

            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = row + x * bpp;

                var rgb = RGB.ToRGB(pixel);
                rgb = rgb.ChangeContrast(factor);
                rgb.SaveToPixel(pixel);
            }
        }
    }
    
    public static unsafe void Negative(BitmapData data)
    {
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;
        

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;

            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = row + x * bpp;

                var rgb = RGB.ToRGB(pixel);
                rgb = rgb.Negative();
                rgb.SaveToPixel(pixel);
            }
        }
    }
}