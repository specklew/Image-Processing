using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using image_processing_core;

namespace task_2;

public static class NonLinearOperations
{
    public static unsafe Bitmap RobertsOpeator(Bitmap bitmap)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = (byte*)data.Scan0 + y * data.Stride;
            for (var x = 0; x < data.Width; x++)
            {
                byte* p = row + x * bpp;
                
                RGB64 rgb00 = RGB64.ToRGB(p);
                RGB64 rgb10 = rgb00;
                RGB64 rgb01 = rgb00;
                RGB64 rgb11 = rgb00;
                
                if(x < data.Width - 1)rgb10 = RGB64.ToRGB(p + bpp);
                if(y < data.Height - 1)rgb01 = RGB64.ToRGB(p + data.Stride);
                if(x < data.Width - 1 && y < data.Height - 1)rgb11 = RGB64.ToRGB(p + data.Stride + bpp);

                rgb00 = (rgb00 - rgb11) * (rgb00 - rgb11) + (rgb01 - rgb10) * (rgb01 - rgb10);
                rgb00 = new RGB64(Math.Sqrt(rgb00.R), Math.Sqrt(rgb00.G), Math.Sqrt(rgb00.B));
                
                rgb00.SaveToPixel((byte*)newData.Scan0 + y * newData.Stride + x * bpp);
            }
        }

        newBitmap.UnlockBits(newData);
        return newBitmap;
    }
    
    public static unsafe Bitmap SobelOpeator(Bitmap bitmap)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = (byte*)data.Scan0 + y * data.Stride;
            for (var x = 0; x < data.Width; x++)
            {
                byte* p = row + x * bpp;
                RGB64 rgb00 = RGB64.ToRGB(p);
                
                RGB64[] A = new RGB64[8];
                for (var i = 0; i < A.Length; i++)
                {
                    A[i] = rgb00;
                }
                
                RGB64 coefficientX;
                RGB64 coefficientY;
                
                //A0, A1, A2
                if(x - 1 > 0 && y - 1 > 0) A[0] = RGB64.ToRGB(p - bpp - data.Stride);
                if(y - 1 > 0) A[1] = RGB64.ToRGB(p - data.Stride);
                if(x + 1 < data.Width && y - 1 > 0) A[2] = RGB64.ToRGB(p + bpp - data.Stride);
                
                //A7, A3
                if(x - 1 > 0 ) A[7] = RGB64.ToRGB(p - bpp);
                if(x + 1 < data.Width ) A[3] = RGB64.ToRGB(p + bpp);
                
                //A6, A5, A4
                if(x - 1 > 0 && y + 1 < data.Height) A[6] = RGB64.ToRGB(p - bpp + data.Stride);
                if(y + 1 < data.Height) A[5] = RGB64.ToRGB(p + data.Stride);
                if(x + 1 < data.Width && y + 1 < data.Height) A[4] = RGB64.ToRGB(p + bpp + data.Stride);

                coefficientX = A[2] + A[3] * 2 + A[4] - A[0] - A[7] * 2 - A[6];
                coefficientY = A[0] + A[1] * 2 + A[2] - A[6] - A[5] * 2 - A[4];

                rgb00 = coefficientX * coefficientX + coefficientY * coefficientY;
                rgb00 = rgb00.Pow(0.5);

                rgb00.SaveToPixel((byte*)newData.Scan0 + y * newData.Stride + x * bpp);
            }
        }

        newBitmap.UnlockBits(newData);
        return newBitmap;
    }

    public static unsafe Bitmap UolisOperator(Bitmap bitmap)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = (byte*)data.Scan0 + y * data.Stride;
            for (var x = 0; x < data.Width; x++)
            {
                byte* p = row + x * bpp;
                RGB64 rgb00 = RGB64.ToRGB(p);
                RGB64 A1 = rgb00;
                RGB64 A3 = rgb00;
                RGB64 A5 = rgb00;
                RGB64 A7 = rgb00;

                if (y > 0) A1 = RGB64.ToRGB(p - data.Stride);
                if (x < data.Width - 1) A3 = RGB64.ToRGB(p + bpp);
                if (y < data.Height - 1) A5 = RGB64.ToRGB(p + data.Stride);
                if (x > 0) A7 = RGB64.ToRGB(p - bpp);

                A1 = A1.AddWhereZero() * A3.AddWhereZero() * A5.AddWhereZero() * A7.AddWhereZero();
                rgb00 = rgb00.AddWhereZero().Pow(4);
                
                if (A1.Length() != 0) rgb00 /= A1;
                rgb00 *= 100000000;
                if (rgb00.Length() != 0) rgb00 = rgb00.LogarithmBase10();
                rgb00 *= 0.25;

                rgb00.SaveToPixel((byte*)newData.Scan0 + y * newData.Stride + x * bpp);
            }
        }
        newBitmap.UnlockBits(newData);
        return newBitmap;
    }

    private static RGB64 LogarithmBase10(this RGB64 rgb)
    {
        return new RGB64(Math.Log10(rgb.R), Math.Log10(rgb.G), Math.Log10(rgb.B));
    }
    
    private const double NormalizationFactor = 255;
    
    public static Bitmap Uolis(Bitmap bitmap)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Height; x++)
            {
                RGB64 rgb = RGB64.ToRGB(bitmap.GetPixel(x, y));
                RGB64 A1 = rgb;
                RGB64 A3 = rgb;
                RGB64 A5 = rgb;
                RGB64 A7 = rgb;

                if (y > 0) A1 = RGB64.ToRGB(bitmap.GetPixel(x, y - 1));
                if (x < bitmap.Width - 1) A3 = RGB64.ToRGB(bitmap.GetPixel(x + 1, y));
                if (y < bitmap.Height - 1) A5 = RGB64.ToRGB(bitmap.GetPixel(x, y + 1));
                if (x > 0) A7 = RGB64.ToRGB(bitmap.GetPixel(x - 1, y));
                
                A1 = A1.AddWhereZero() * A3.AddWhereZero() * A5.AddWhereZero() * A7.AddWhereZero();
                rgb = rgb.Pow(4);
                
                rgb /= A1;
                rgb = rgb.Log(10);
                rgb *= NormalizationFactor * 0.25;
                newBitmap.SetPixel(x, y, rgb.ToColor());

            }
        }

        return newBitmap;
    }

    private static RGB64 Log(this RGB64 rgb, double newBase = 2)
    {
        return new RGB64(Math.Log(rgb.R, newBase), Math.Log(rgb.G, newBase), Math.Log(rgb.B, newBase));
    }
}