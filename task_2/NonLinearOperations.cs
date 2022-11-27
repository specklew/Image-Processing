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
}