using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using image_processing_core;

namespace task_3;

public static class ImageSegmentation
{
    private static readonly (int, int)[] Offset = { 
        (-1, -1), 
        ( 0, -1),
        ( 1, -1),
            
        (-1,  0),
        ( 1,  0),
            
        (-1,  1),
        ( 0,  1),
        ( 1,  1) 
    };
    
    public static unsafe Bitmap GrowRegion(Bitmap bitmap, int seedX, int seedY, int tolerance)
    {
        var result = (bitmap.Clone() as Bitmap)!;
        BitmapData data = ImageIO.LockPixels(result);
        
        List<(int, int)> region = GetRegion(bitmap, seedX, seedY, tolerance);
        for (var i = 0; i < region.Count; i++)
        {
            (int x, int y) = region[i];
            byte* pixel = (byte*)data.Scan0 + x * (data.Stride / data.Width) + y * data.Stride;
            RGB64.Black.SaveToPixel(pixel);
        }

        result.UnlockBits(data);
        return result;
    }
    
    //TODO: Find a better way to optimize.
    private static unsafe List<(int, int)> GetRegion(Bitmap bitmap, int seedX, int seedY, int tolerance)
    {
        BitmapData data = ImageIO.LockPixels(bitmap);
        
        var regionPixels = new List<(int, int)>();
        
        var pixelsToProcess = new Queue<(int, int)>();
        pixelsToProcess.Enqueue((seedX, seedY));
        
        regionPixels.Add((seedX, seedY));

        while (pixelsToProcess.Count > 0)
        {
            (int x, int y) = pixelsToProcess.Dequeue();
            
            byte* pixelXY = (byte*)data.Scan0 + x * (data.Stride / data.Width) + y * data.Stride;
            RGB64 rgbXY = RGB64.ToRGB(pixelXY);

            var pixels = new (int, int)[8];

            Parallel.For(0, 7, k =>
            {
                (int i, int j) = Offset[k];
                i += x;
                j += y;

                if (i < 0 || i >= data.Width || j < 0 || j >= data.Height) return;
                
                byte* pixelIJ = (byte*)data.Scan0 + i * (data.Stride / data.Width) + j * data.Stride;
                
                RGB64 rgbIJ = RGB64.ToRGB(pixelIJ);

                if (Math.Abs((rgbIJ - rgbXY).Length()) <= tolerance)
                {
                    pixels[k] = (i, j);
                }
            });

            foreach ((int, int) pixel in pixels)
            {
                if (regionPixels.Contains(pixel)) continue;
                
                regionPixels.Add(pixel);
                pixelsToProcess.Enqueue(pixel);
            }
        }

        return regionPixels;
    }
}