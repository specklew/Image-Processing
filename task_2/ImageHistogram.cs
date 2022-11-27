using System.Drawing;
using System.Drawing.Drawing2D;
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
        Color red = Color.FromArgb(255, 255, 0, 0);
        Color green = Color.FromArgb(128, 0, 255, 0);
        Color blue = Color.FromArgb(85, 0, 0, 255);
        
        var bucketsAndPens = new List<Tuple<IReadOnlyList<int>, Brush>>
        {
            new(_redBucket, new SolidBrush(red)),
            new(_greenBucket, new SolidBrush(green)),
            new(_blueBucket, new SolidBrush(blue))
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
        float scale = 1024f / bucket.Max();
        
        Bitmap bitmap = new Bitmap(1024, 1024, PixelFormat.Format24bppRgb);

        Graphics graphics = Graphics.FromImage(bitmap);
        DrawHistogramGraphics(graphics, bucket, scale, Brushes.White);
        
        return bitmap;
    }
    
    private static Bitmap DrawHistogramFromBucket(List<Tuple<IReadOnlyList<int>, Brush>> bucketsAndPens)
    {
        int max = bucketsAndPens.Select(tuple => tuple.Item1.Max()).Prepend(0).Max();
        float scale = 1024f / max;

        var bitmap = new Bitmap(1024, 1024, PixelFormat.Format24bppRgb);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.CompositingQuality = CompositingQuality.HighSpeed;
        
        foreach ((IReadOnlyList<int>? bucket, Brush? brush) in bucketsAndPens)
        {
            DrawHistogramGraphics(graphics, bucket, scale, brush);
        }
        
        return bitmap;
    }

    private static void DrawHistogramGraphics(Graphics graphics, IReadOnlyList<int> bucket, float scale, Brush? brush = null)
    {
        brush ??= new SolidBrush(Color.White);
        
        var points = new Point[258];

        for (var i = 1; i < 257; i++)
        {
            var p = new Point(i * 4, 1024 - (int)(bucket[i - 1] * scale));
            points[i] = p;
        }

        points[0] = new Point(0, 1024);
        points[257] = new Point(1024, 1024);
        

        graphics.FillPolygon(brush, points);
    }
}