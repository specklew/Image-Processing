using System.Drawing.Imaging;
using System.Numerics;
using image_processing_core;

namespace task_1;

public static class AnalysisOperations
{
    public static unsafe Vector3 MeanSquareError(BitmapData data1, BitmapData data2)
    {
        var pt1 = (byte*)data1.Scan0;
        var pt2 = (byte*)data2.Scan0;
        
        int bpp1 = data1.Stride / data1.Width;
        int bpp2 = data2.Stride / data2.Width;

        RGB sum = RGB.Zero();
        
        for (var y = 0; y < data1.Height; y++)
        {
            byte* row1 = pt1 + y * data1.Stride;
            byte* row2 = pt2 + y * data2.Stride;

            for (var x = 0; x < data1.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);

                rgb1 -= rgb2;
                rgb1 ^= 2;
                sum += rgb1;
            }
        }

        Vector3 result = sum.ToVector3() / (data1.Width * data1.Height);
        return result;
    }
    
    public static unsafe Vector3 PeakMeanSquareError(BitmapData data1, BitmapData data2)
    {
        var pt1 = (byte*)data1.Scan0;
        var pt2 = (byte*)data2.Scan0;
        
        int bpp1 = data1.Stride / data1.Width;
        int bpp2 = data2.Stride / data2.Width;

        RGB sum = RGB.Zero();
        RGB max = RGB.Zero();
        
        for (var y = 0; y < data1.Height; y++)
        {
            byte* row1 = pt1 + y * data1.Stride;
            byte* row2 = pt2 + y * data2.Stride;

            for (var x = 0; x < data1.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);

                if (rgb1 > max) max = rgb1;
                
                rgb1 -= rgb2;
                rgb1 ^= 2;
                sum += rgb1;
            }
        }

        Vector3 result = sum.ToVector3() / (data1.Width * data1.Height);
        result /= max.ToVector3();
        
        return result;
    }
    
    public static unsafe Vector3 SignalNoiseRatio(BitmapData data1, BitmapData data2)
    {
        var pt1 = (byte*)data1.Scan0;
        var pt2 = (byte*)data2.Scan0;
        
        int bpp1 = data1.Stride / data1.Width;
        int bpp2 = data2.Stride / data2.Width;

        RGB initialSum = RGB.Zero();
        RGB sum = RGB.Zero();
        
        for (var y = 0; y < data1.Height; y++)
        {
            byte* row1 = pt1 + y * data1.Stride;
            byte* row2 = pt2 + y * data2.Stride;

            for (var x = 0; x < data1.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);
                
                initialSum += rgb1 ^ 2;

                rgb1 -= rgb2;
                rgb1 ^= 2;
                sum += rgb1;
            }
        }

        Vector3 result = initialSum.ToVector3() / sum.ToVector3();
        result = Log10(result);
        return result;
    }
    
    public static unsafe Vector3 PeakSignalNoiseRatio(BitmapData data1, BitmapData data2)
    {
        var pt1 = (byte*)data1.Scan0;
        var pt2 = (byte*)data2.Scan0;
        
        int bpp1 = data1.Stride / data1.Width;
        int bpp2 = data2.Stride / data2.Width;

        RGB max = RGB.Zero();
        RGB sum = RGB.Zero();
        
        for (var y = 0; y < data1.Height; y++)
        {
            byte* row1 = pt1 + y * data1.Stride;
            byte* row2 = pt2 + y * data2.Stride;

            for (var x = 0; x < data1.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);

                if (rgb1 > max) max = rgb1;
                
                rgb1 -= rgb2;
                rgb1 ^= 2;
                sum += rgb1;
            }
        }

        Vector3 result = max.ToVector3() / sum.ToVector3();
        result = Log10(result);
        return result;
    }
    
    public static unsafe Vector3 MaxDifference(BitmapData data1, BitmapData data2)
    {
        var pt1 = (byte*)data1.Scan0;
        var pt2 = (byte*)data2.Scan0;
        
        int bpp1 = data1.Stride / data1.Width;
        int bpp2 = data2.Stride / data2.Width;

        RGB max = RGB.Zero();

        for (var y = 0; y < data1.Height; y++)
        {
            byte* row1 = pt1 + y * data1.Stride;
            byte* row2 = pt2 + y * data2.Stride;

            for (var x = 0; x < data1.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);
                
                rgb1 -= rgb2;
                rgb1 = rgb1.AbsoluteValue();
                
                if (rgb1 > max) max = rgb1;
            }
        }

        var result = max.ToVector3();
        return result;
    }

    private static Vector3 Log10(Vector3 vector)
    {
        var result = new Vector3((float)Math.Log10(vector.X), (float)Math.Log10(vector.X), (float)Math.Log10(vector.X));
        return result * 10;
    }
}