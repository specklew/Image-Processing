using System.Drawing;
using System.Drawing.Imaging;
using image_processing_core;

namespace task_2;

public class ImageHistogram
{
    public int[] RedBucket => _redBucket;
    public int[] GreenBucket => _greenBucket;
    public int[] BlueBucket => _blueBucket;

    public Dictionary<Channel, int[]> Buckets =>
        new()
        { 
            { Channel.Red, _redBucket }, 
            { Channel.Green , _greenBucket},
            { Channel.Blue , _blueBucket}
        };

    private readonly int[] _redBucket = new int[256];
    private readonly int[] _greenBucket = new int[256];
    private readonly int[] _blueBucket = new int[256];

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
                _redBucket[rgb.R]++;
                _greenBucket[rgb.G]++;
                _blueBucket[rgb.B]++;
            }
        }
    }

    public Bitmap GetHistogram()
    {
        var bucketsAndPens = new List<Tuple<IReadOnlyList<int>, Pen>>
        {
            new(_redBucket, Pens.Red),
            new(_greenBucket, Pens.Green),
            new(_blueBucket, Pens.Blue)
        };
        
        return DrawHistogramFromBucket(bucketsAndPens);
    }
    
    public Bitmap GetRedHistogram()
    {
        return DrawHistogramFromBucket(_redBucket);
    }
    
    public Bitmap GetGreenHistogram()
    {
        return DrawHistogramFromBucket(_greenBucket);
    }
    
    public Bitmap GetBlueHistogram()
    {
        return DrawHistogramFromBucket(_blueBucket);
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
        var points = new Point[256];
        
        for (var i = 0; i < 256; i++)
        {
            var p = new Point(i * scale, bitmap.Height - bucket[i]);
            points[i] = p;
        }
        
        for (var x = 0; x < bitmap.Width - 1; x++)
        {
            graphics.DrawLines(pen, points);
        }
    }
}