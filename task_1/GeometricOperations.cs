using System.Drawing.Imaging;
using image_processing_core;

namespace task_1;

public static class GeometricOperations
{
    public static unsafe void VerticalFlip(BitmapData data)
    {
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height / 2; y++)
        {
            byte* r1 = pt + y * data.Stride;
            byte* r2 = pt + (data.Height - y - 1) * data.Stride;

            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel1 = r1 + x * bpp;
                byte* pixel2 = r2 + x * bpp;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);
                
                rgb2.SaveToPixel(pixel1);
                rgb1.SaveToPixel(pixel2);
            }
        }
    }
}