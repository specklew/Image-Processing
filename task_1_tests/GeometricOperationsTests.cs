using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using task_1;

namespace task_1_tests;

[TestFixture]
public class GeometricOperationsTests
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\lenna";
    private const string SavePath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\tests\\geometric";

    private Bitmap _bitmap = null!;
    private BitmapData _data = null!;


    [SetUp]
    public void Setup()
    {
        _bitmap = ImageIO.LoadImage($"{TestPath}\\original.bmp");

        _data = ImageIO.LockPixels(_bitmap);
    }
    
    [Test]
    public void HorizontalFlipTest()
    {
        GeometricOperations.HorizontalFlip(_data);
    }
    
    [Test]
    public void VerticalFlipTest()
    {
        GeometricOperations.VerticalFlip(_data);
    }
    
    [Test]
    public void DiagonalFlipTest()
    {
        GeometricOperations.DiagonalFlip(_data);
    }
    
    [Test]
    public void ShrinkingTest()
    {
        GeometricOperations.Resize(ref _bitmap, _data, 0.3f);
    }
    
    [Test]
    public void EnlargingTest()
    {
        GeometricOperations.Resize(ref _bitmap, _data, 3f);
    }
    
    [TearDown]
    public void TearDown()
    {
        _bitmap.UnlockBits(_data);
        Console.WriteLine($"Saving current operation under: {SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp\n");
        ImageIO.SaveImage(_bitmap, $"{SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp");
    }
}