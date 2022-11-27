using System.Drawing;
using System.Drawing.Imaging;

namespace image_processing;

public static class ImageIO
{
    public static Bitmap LoadImage(string name)
    {
        try
        {
            return PaintOn24bpp(Bitmap.FromFile(name));
        }
        catch (InvalidOperationException exception)
        {
            Console.Write("The image your trying to read does not exist!\n" + exception.Message);
            return null;
        }
    }
    
    public static Bitmap PaintOn24bpp(Image image)
    {
        Bitmap bp = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
        using (Graphics gr = Graphics.FromImage(bp))
            gr.DrawImage(image, new Rectangle(0, 0, bp.Width, bp.Height));
        return bp;
    }
    
    public static BitmapData LockPixels(Bitmap bitmap)
    {
        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite,
            bitmap.PixelFormat);
        return data;
    }

    public static void SaveImage(Bitmap image, string path)
    {
        image.Save(path);
    }
}