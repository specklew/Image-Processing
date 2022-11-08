using System.Drawing;
using System.Drawing.Imaging;
using image_processing_core;

namespace task_2;

public class ImageHistogram
{
    private int[] redBucket = new int[256];
    private int[] greenBucket = new int[256];
    private int[] blueBucket = new int[256];

    public ImageHistogram(BitmapData data)
    {
        Translate(data);
    }

    private unsafe void Translate(BitmapData data)
    {
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;
        
        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;

            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = row + x * bpp;

                var rgb = RGB.ToRGB(pixel);
                redBucket[rgb.R]++;
                greenBucket[rgb.G]++;
                blueBucket[rgb.B]++;
            }
        }
    }
    
    public Bitmap GetHistogram()
    {
        var bucketsAndPens = new List<Tuple<IReadOnlyList<int>, Pen>>();
        bucketsAndPens.Add(new Tuple<IReadOnlyList<int>, Pen>(redBucket, Pens.Red));
        bucketsAndPens.Add(new Tuple<IReadOnlyList<int>, Pen>(greenBucket, Pens.Green));
        bucketsAndPens.Add(new Tuple<IReadOnlyList<int>, Pen>(blueBucket, Pens.Blue));
        return DrawHistogramFromBucket(bucketsAndPens);
    }
    
    public Bitmap GetRedHistogram()
    {
        return DrawHistogramFromBucket(redBucket);
    }
    
    public Bitmap GetGreenHistogram()
    {
        return DrawHistogramFromBucket(greenBucket);
    }
    
    public Bitmap GetBlueHistogram()
    {
        return DrawHistogramFromBucket(blueBucket);
    }

    private static Bitmap DrawHistogramFromBucket(IReadOnlyList<int> bucket)
    {
        int scale = bucket.Max() / 256;
        
        Bitmap bitmap = new Bitmap(256 * scale, bucket.Max(), PixelFormat.Format24bppRgb);

        DrawHistogramGraphics(bitmap, bucket);
        
        return bitmap;
    }
    
    private static Bitmap DrawHistogramFromBucket(List<Tuple<IReadOnlyList<int>, Pen>> bucketsAndPens)
    {
        int max = bucketsAndPens.Select(tuple => tuple.Item1.Max()).Prepend(0).Max();
        int scale = max / 256;

        var bitmap = new Bitmap(256 * scale, max, PixelFormat.Format24bppRgb);
        foreach ((IReadOnlyList<int>? bucket, Pen? pen) in bucketsAndPens)
        {
            DrawHistogramGraphics(bitmap, bucket, pen);
        }
        
        return bitmap;
    }

    private static void DrawHistogramGraphics(Bitmap bitmap, IReadOnlyList<int> bucket, Pen? pen = null)
    {
        pen ??= Pens.White;
        int scale = bitmap.Width / 256;
        
        Graphics graphics = Graphics.FromImage(bitmap);
        Point[] points = new Point[256];
        
        for (int i = 0; i < 256; i++)
        {
            Point p = new Point(i * scale, bitmap.Height - bucket[i]);
            points[i] = p;
        }
        
        for (int x = 0; x < bitmap.Width - 1; x++)
        {
            graphics.DrawLines(pen, points);
        }
    }
}