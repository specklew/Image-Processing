using System.Drawing.Imaging;
using image_processing_core;

namespace task_1;

public static class AnalysisOperations
{
    public static unsafe RGB64 MeanSquareError(BitmapData inputData, BitmapData outputData)
    {
        var pt1 = (byte*)inputData.Scan0;
        var pt2 = (byte*)outputData.Scan0;
        
        int bpp1 = inputData.Stride / inputData.Width;
        int bpp2 = outputData.Stride / outputData.Width;

        RGB64 sum = new RGB64(0, 0, 0);
        
        for (var y = 0; y < inputData.Height; y++)
        {
            byte* row1 = pt1 + y * inputData.Stride;
            byte* row2 = pt2 + y * outputData.Stride;

            for (var x = 0; x < inputData.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);

                rgb1 -= rgb2;
                rgb1 *= rgb1;
                sum += rgb1;
            }
        }

        RGB64 result = sum / (inputData.Width * inputData.Height);
        return result;
    }
    
    public static unsafe RGB64 PeakMeanSquareError(BitmapData inputData, BitmapData outputData)
    {
        var pt1 = (byte*)inputData.Scan0;
        var pt2 = (byte*)outputData.Scan0;
        
        int bpp1 = inputData.Stride / inputData.Width;
        int bpp2 = outputData.Stride / outputData.Width;

        RGB64 sum = new RGB64(0, 0, 0);
        RGB max = RGB.Zero();
        
        for (var y = 0; y < inputData.Height; y++)
        {
            byte* row1 = pt1 + y * inputData.Stride;
            byte* row2 = pt2 + y * outputData.Stride;

            for (var x = 0; x < inputData.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);

                if (rgb1 > max) max = rgb1;
                
                rgb1 -= rgb2;
                rgb1 *= rgb1;
                sum += rgb1;
            }
        }
        
        max *= max;
        RGB64 result = sum / (inputData.Width * inputData.Height);
        result /= max;

        return result;
    }
    
    public static unsafe RGB64 SignalNoiseRatio(BitmapData inputData, BitmapData outputData)
    {
        var pt1 = (byte*)inputData.Scan0;
        var pt2 = (byte*)outputData.Scan0;
        
        int bpp1 = inputData.Stride / inputData.Width;
        int bpp2 = outputData.Stride / outputData.Width;

        RGB64 initialSum = new RGB64(0, 0, 0);
        RGB64 sum = new RGB64(0, 0, 0);
        
        for (var y = 0; y < inputData.Height; y++)
        {
            byte* row1 = pt1 + y * inputData.Stride;
            byte* row2 = pt2 + y * outputData.Stride;

            for (var x = 0; x < inputData.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                RGB64 rgb1 = RGB64.ToRGB(pixel1);
                RGB64 rgb2 = RGB64.ToRGB(pixel2);
                
                initialSum += rgb1 * rgb1;

                rgb1 -= rgb2;
                rgb1 *= rgb1;
                sum += rgb1;
            }
        }

        RGB64 result = initialSum / sum;
        result = Log10(result);
        return result;
    }
    
    public static unsafe RGB64 PeakSignalNoiseRatio(BitmapData inputData, BitmapData outputData)
    {
        var pt1 = (byte*)inputData.Scan0;
        var pt2 = (byte*)outputData.Scan0;
        
        int bpp1 = inputData.Stride / inputData.Width;
        int bpp2 = outputData.Stride / outputData.Width;

        RGB64 max = new RGB64(0, 0, 0);
        RGB64 sum = new RGB64(0, 0, 0);
        
        for (var y = 0; y < inputData.Height; y++)
        {
            byte* row1 = pt1 + y * inputData.Stride;
            byte* row2 = pt2 + y * outputData.Stride;

            for (var x = 0; x < inputData.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB64.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);

                if (rgb1 > max) max = rgb1;
                
                rgb1 -= rgb2;
                rgb1 *= rgb1;
                sum += rgb1;
            }
        }

        max *= max;
        max *= inputData.Width * inputData.Height;
        RGB64 result = max / sum;
        result = Log10(result);
        return result;
    }
    
    public static unsafe RGB64 MaxDifference(BitmapData inputData, BitmapData outputData)
    {
        var pt1 = (byte*)inputData.Scan0;
        var pt2 = (byte*)outputData.Scan0;
        
        int bpp1 = inputData.Stride / inputData.Width;
        int bpp2 = outputData.Stride / outputData.Width;

        RGB64 max =  new RGB64(0,0,0);

        for (var y = 0; y < inputData.Height; y++)
        {
            byte* row1 = pt1 + y * inputData.Stride;
            byte* row2 = pt2 + y * outputData.Stride;

            for (var x = 0; x < inputData.Width; x++)
            {
                byte* pixel1 = row1 + x * bpp1;
                byte* pixel2 = row2 + x * bpp2;

                var rgb1 = RGB64.ToRGB(pixel1);
                var rgb2 = RGB64.ToRGB(pixel2);
                
                rgb1 -= rgb2;
                rgb1 = rgb1.AbsoluteValue();
                
                if (rgb1 > max) max = rgb1;
            }
        }
        
        return max;
    }

    private static RGB64 Log10(RGB64 rgb)
    {
        var result = new RGB64(Math.Log10(rgb.R), Math.Log10(rgb.G), Math.Log10(rgb.B));
        return result * 10;
    }
}