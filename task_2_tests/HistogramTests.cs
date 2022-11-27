using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using task_2;

namespace task_2_tests;

[TestFixture]
public class HistogramTests
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\lenna";
    private const string SavePath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\tests\\histogram";
    
    private Bitmap _bitmap = null!;
    private Bitmap _histogramBmp = null!;
    private BitmapData _data = null!;
    
    [SetUp]
    public void Setup()
    {
        _bitmap = ImageIO.LoadImage($"{TestPath}\\original.bmp");
        _data = ImageIO.LockPixels(_bitmap);
    }

    [Test]
    public void RGBHistogramTest()
    {
        ImageHistogram histogram = new ImageHistogram(_data);
        _histogramBmp = histogram.GetHistogram();
    }

    [TearDown]
    public void TearDown()
    {
        _bitmap.UnlockBits(_data);
        Console.WriteLine($"Saving current operation under: {SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp\n");
        ImageIO.SaveImage(_histogramBmp, $"{SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp");
    }
}