using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using task_1;

namespace task_1_tests;

[TestFixture]
public class ElementaryOperationsTests
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\lenna";
    private const string SavePath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\tests\\elementary";

    private Bitmap _bitmap = null!;
    private BitmapData _data = null!;


    [SetUp]
    public void Setup()
    {
        _bitmap = ImageIO.LoadImage($"{TestPath}\\original.bmp");

        _data = ImageIO.LockPixels(_bitmap);
    }
    
    [Test]
    public void BrightnessPlus50Test()
    {
        ElementaryOperations.ModifyBrightness(_data, 50);
    }
    
    [Test]
    public void BrightnessMinus50Test()
    {
        ElementaryOperations.ModifyBrightness(_data, -50);
    }
    
    [Test]
    public void ContrastPlus100Test()
    {
        ElementaryOperations.ModifyContrast(_data, 100);
    }
    
    [Test]
    public void ContrastMinus100Test()
    {
        ElementaryOperations.ModifyContrast(_data, -100);
    }
    
    [Test]
    public void NegativeTest()
    {
        ElementaryOperations.Negative(_data);
    }
    
    [TearDown]
    public void TearDown()
    {
        _bitmap.UnlockBits(_data);
        Console.WriteLine($"Saving current operation under: {SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp\n");
        ImageIO.SaveImage(_bitmap, $"{SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp");
    }

}