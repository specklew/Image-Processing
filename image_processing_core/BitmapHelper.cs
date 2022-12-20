using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using image_processing;

namespace image_processing_core;

public static class BitmapHelper
{
    public static bool Compare1Bpp(Bitmap bmp1, Bitmap bmp2)
    {
        Bitmap clone1 = ImageIO.PaintOn1bpp(bmp1);
        Bitmap clone2 = ImageIO.PaintOn1bpp(bmp2);
        BitmapData bmpData1 = clone1.LockBits(new Rectangle(Point.Empty, bmp1.Size), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
        BitmapData bmpData2 = clone2.LockBits(new Rectangle(Point.Empty, bmp2.Size), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);

        try
        {
            int numBytes = bmpData1.Stride * bmp1.Height;
            IntPtr bmpPtr1 = bmpData1.Scan0;
            IntPtr bmpPtr2 = bmpData2.Scan0;

            for (int i = 0; i < numBytes; i++)
            {
                if (Marshal.ReadByte(bmpPtr1, i) != Marshal.ReadByte(bmpPtr2, i))
                {
                    return false;
                }
            }

            return true;
        }
        finally
        {
            clone1.UnlockBits(bmpData1);
            clone2.UnlockBits(bmpData2);
        }
    }
}