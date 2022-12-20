using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using image_processing;
using image_processing_core;

namespace task_3;

public static class BasicMorphologicalOperations
{

    public static unsafe Bitmap Erode(Bitmap bitmap, int[,] kernel)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        int kernelRadius = kernel.GetLength(0) / 2;

        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                var shouldErode = false;

                for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                {
                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        int pixelX = kx + x;
                        int pixelY = ky + y;

                        if (pixelX < 0 || pixelX >= data.Width || pixelY < 0 || pixelY >= data.Height) continue;

                        byte* pixel = pt + data.Stride * pixelY + bpp * pixelX;

                        RGB64 pixelColor = RGB64.ToRGB(pixel);

                        if (kernel[ky + kernelRadius, kx + kernelRadius] == 1 && pixelColor == RGB64.Black)
                        {
                            shouldErode = true;
                            break;
                        }
                    }

                    if (shouldErode)
                    {
                        break;
                    }
                }

                if (shouldErode)
                {
                    RGB64.Black.SaveToPixel((byte*)newData.Scan0 + bpp * x + newData.Stride * y);
                }
            }
        }

        newBitmap.UnlockBits(newData);
        return newBitmap;
    }

    public static unsafe Bitmap Dilate(Bitmap bitmap, int[,] kernel)
    {
        var newBitmap = (bitmap.Clone() as Bitmap)!;

        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        int kernelRadius = kernel.GetLength(0) / 2;

        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        Parallel.For(0, data.Height, y =>
        {
            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = pt + data.Stride * y + bpp * x;
                
                if(pixel[0] != 255) continue;

                for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                {
                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        int pixelX = kx + x;
                        int pixelY = ky + y;

                        if (pixelX < 0 || pixelX >= data.Width || pixelY < 0 || pixelY >= data.Height) continue;
                        
                        if(kernel[kx + kernelRadius, ky + kernelRadius] == 1) RGB64.White.SaveToPixel((byte*)newData.Scan0 + bpp * pixelX + newData.Stride * pixelY);
                    }
                }
            }
        });

        bitmap.UnlockBits(data);
        newBitmap.UnlockBits(newData);
        return newBitmap;
    }

    public static Bitmap Opening(Bitmap bitmap, int[,] kernel)
    {
        bitmap = Erode(bitmap, kernel);
        bitmap = Dilate(bitmap, kernel);
        return bitmap;
    }
    
    public static Bitmap Closing(Bitmap bitmap, int[,] kernel)
    {
        bitmap = Dilate(bitmap, kernel);
        bitmap = Erode(bitmap, kernel);
        return bitmap;
    }

    public static unsafe Bitmap HitOrMiss(Bitmap bitmap, int[,] kernel)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);
        
        int kernelRadius = kernel.GetLength(0) / 2;
        
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;
        
        for (var x = 0; x < data.Width; x++)
        {
            for (var y = 0; y < data.Height; y++)
            {
                var match = true;
                for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                {
                    for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                    {
                        int pixelX = x + kx;
                        int pixelY = y + ky;

                        if (pixelX < 0 || pixelX >= data.Width || pixelY < 0 || pixelY >= data.Height) continue;
                        
                        byte* pixel = pt + data.Stride * pixelY + bpp * pixelX;
                        RGB64 rgb = RGB64.ToRGB(pixel);
                        
                        if (rgb == RGB64.White && kernel[kx + 1, ky + 1] == 1)
                        {
                            match = false;
                            break;
                        }

                        if (rgb == RGB64.Black && kernel[kx + 1, ky + 1] == 0)
                        {
                            match = false;
                            break;
                        }
                    }
                    if (!match)
                    {
                        break;
                    }
                }
                
                if (match)
                {
                    RGB64.White.SaveToPixel((byte*)newData.Scan0 + bpp * x + newData.Stride * y);
                }
                else
                {
                    RGB64.Black.SaveToPixel((byte*)newData.Scan0 + bpp * x + newData.Stride * y);
                }
            }
        }

        newBitmap.UnlockBits(newData);
        return newBitmap;
    }

    public static Bitmap Intersection(Bitmap bitmap1, Bitmap bitmap2)
    {
        var result = bitmap1.Clone() as Bitmap;
        BitmapData data1 = bitmap1.LockBits(new Rectangle(Point.Empty, bitmap1.Size), ImageLockMode.ReadOnly, bitmap1.PixelFormat);
        BitmapData data2 = bitmap2.LockBits(new Rectangle(Point.Empty, bitmap2.Size), ImageLockMode.ReadOnly, bitmap2.PixelFormat);
        BitmapData resultData = result!.LockBits(new Rectangle(Point.Empty, result.Size), ImageLockMode.WriteOnly, result.PixelFormat);

        int bytes = data1.Height * data1.Stride;

        Parallel.For(0, bytes, i =>
        {
            byte b1 = Marshal.ReadByte(data1.Scan0 + i);
            byte b2 = Marshal.ReadByte(data2.Scan0 + i);

            if (b1 == b2)
            {
                Marshal.WriteByte(resultData.Scan0 + i, b1);
            }
            else
            {
                Marshal.WriteByte(resultData.Scan0 + i, 0);
            }
        });

        bitmap1.UnlockBits(data1);
        bitmap2.UnlockBits(data2);
        result.UnlockBits(resultData);
        return result;
    }

    public static Bitmap M3(Bitmap bitmap, int[,] kernel, int x, int y)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        Bitmap result = new(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
        result.SetPixel(x, y, Color.White);

        Bitmap last;
        var i = 0;
        
        do
        {
            last = (result.Clone() as Bitmap)!;
            result = Intersection(Dilate(last, kernel), bitmap);
            if (i++ % 10 == 0)
            {
                Console.WriteLine(i);
            }

        } while (!BitmapHelper.Compare1Bpp(result, last));

        stopwatch.Stop();
        Console.WriteLine("Elapsed = " + stopwatch.ElapsedMilliseconds + " ms");

        return result;
    }
}