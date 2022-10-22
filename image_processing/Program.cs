using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using CommandLine;
using task_1;

namespace image_processing;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(t =>
        {
            //If parsing is successful.
            Bitmap bitmap = ImageIO.LoadImage(t.FilePath);
            BitmapData data = ImageIO.LockPixels(bitmap);
            
            if (t.Brightness != 0)
            {
                ElementaryOperations.ModifyBrightness(data, t.Brightness);
            }
            if (t.Contrast != 0)
            {
                ElementaryOperations.ModifyContrast(data, t.Contrast);
            }
            if (t.Negative)
            {
                ElementaryOperations.Negative(data);   
            }
            if (t.VerticalFlip)
            {
                GeometricOperations.VerticalFlip(data);   
            }
            
            bitmap.UnlockBits(data);
            
            ImageIO.SaveImage(bitmap, "C:\\Studia\\2022_Winter\\Image Processing\\Tinkering\\result.bmp");  
        });
    }
}