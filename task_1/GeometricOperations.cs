using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing;
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
    
    public static unsafe void HorizontalFlip(BitmapData data)
    {
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;

            for (var x = 0; x < data.Width / 2; x++)
            {
                byte* pixel1 = row + x * bpp;
                byte* pixel2 = row + (data.Width - x - 1) * bpp;

                var rgb1 = RGB.ToRGB(pixel1);
                var rgb2 = RGB.ToRGB(pixel2);
                
                rgb2.SaveToPixel(pixel1);
                rgb1.SaveToPixel(pixel2);
            }
        }
    }
    
    public static void DiagonalFlip(BitmapData data)
    {
        VerticalFlip(data);
        HorizontalFlip(data);
    }

    //TODO: Rewrite using bit locking.
    public static void Resize(ref Bitmap bitmap, BitmapData data, float scale)
    {
        bitmap.UnlockBits(data);

        var width = (int)(bitmap.Width * scale);
        var height = (int)(bitmap.Height * scale);
        
        var newBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                float initialX = (float)i / width * (bitmap.Width - 1);
                float initialY = (float)j / height * (bitmap.Height - 1);
                
                var zeroX = (int)initialX;
                var zeroY = (int)initialY;

                float tX = initialX - zeroX;
                float tY = initialY - zeroY;

                //TODO: Change these if statements to switch.
                if (tX == 0 && tY == 0)
                {
                    newBitmap.SetPixel(i, j, bitmap.GetPixel(zeroX, zeroY));
                    continue;
                }
                if (tX == 0)
                {
                    var p0 = RGB.ToRGB(bitmap.GetPixel(zeroX, zeroY));
                    var p1 = RGB.ToRGB(bitmap.GetPixel(zeroX, zeroY + 1));
                    
                    RGB result = MathHelper.LerpRGB(p0, p1, tY);
                    newBitmap.SetPixel(i, j, result.ToColor());
                    continue;
                }
                if (tY == 0)
                {
                    var p0 = RGB.ToRGB(bitmap.GetPixel(zeroX, zeroY));
                    var p1 = RGB.ToRGB(bitmap.GetPixel(zeroX + 1, zeroY));
                    
                    RGB result = MathHelper.LerpRGB(p0, p1, tX);
                    newBitmap.SetPixel(i, j, result.ToColor());
                    continue;
                }

                var p00 = RGB.ToRGB(bitmap.GetPixel(zeroX, zeroY));
                var p01 = RGB.ToRGB(bitmap.GetPixel(zeroX, zeroY + 1));
                var p10 = RGB.ToRGB(bitmap.GetPixel(zeroX + 1, zeroY));
                var p11 = RGB.ToRGB(bitmap.GetPixel(zeroX + 1, zeroY + 1));

                RGB resultRGB = MathHelper.BilinearInterpolation(p00, p01, p10, p11, new Vector2(tX, tY));
                newBitmap.SetPixel(i, j, resultRGB.ToColor());
            }
        }
        
        bitmap = newBitmap;
        ImageIO.LockPixels(bitmap);
    }
}