using System.Drawing;
using System.Drawing.Imaging;

namespace image_processing;

public static class ImageIO
{
    public static Bitmap LoadImage(string name)
    {
        try
        {
            return Bitmap.FromFile(name) as Bitmap;
        }
        catch (InvalidOperationException exception)
        {
            Console.Write("The image your trying to read does not exist!\n" + exception.Message);
            return null;
        }
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