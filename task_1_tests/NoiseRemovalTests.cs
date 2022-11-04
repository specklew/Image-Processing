using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing;
using task_1;

namespace task_1_tests;

[TestFixture]
public class NoiseRemovalTests
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\lenna";
    
    private Bitmap _bitmap = null!;
    private Bitmap _noise = null!;
    private Bitmap _original = null!;
    
    private BitmapData _data = null!;
    private BitmapData _noiseData = null!;
    private BitmapData _originalData = null!;
    
    

    [SetUp]
    public void Setup()
    {
        _bitmap = ImageIO.LoadImage($"{TestPath}\\noise.bmp");
        _noise = ImageIO.LoadImage($"{TestPath}\\noise.bmp");
        _original = ImageIO.LoadImage($"{TestPath}\\original.bmp");
        
        _data = ImageIO.LockPixels(_bitmap);
        _noiseData = ImageIO.LockPixels(_noise);
        _originalData = ImageIO.LockPixels(_original);
    }

    [Test]
    public void HarmonicMeanTest()
    {
        NoiseRemoval.HarmonicFilter(_data, new Rectangle(0, 0, 3, 3));
    }
    
    [Test]
    public void MedianTest()
    {
        NoiseRemoval.MedianFilter(_data, new Rectangle(0, 0, 3, 3));
    }
    
    [Test]
    public void ArithmeticMeanTest()
    {
        NoiseRemoval.ArithmeticMeanFilter(_data, new Rectangle(0, 0, 3, 3));
    }
    
    [Test]
    public void MidpointTest()
    {
        NoiseRemoval.MidpointFilter(_data, new Rectangle(0, 0, 3, 3));
    }
    
    [TearDown]
    public void TearDown()
    {
        Vector3 initialMse = AnalysisOperations.MeanSquareError(_originalData, _noiseData);
        Vector3 initialPmse = AnalysisOperations.PeakMeanSquareError(_originalData, _noiseData);
        Vector3 initialSnr = AnalysisOperations.SignalNoiseRatio(_originalData, _noiseData);
        Vector3 initialPsnr = AnalysisOperations.PeakSignalNoiseRatio(_originalData, _noiseData);
        Vector3 initialMd = AnalysisOperations.MaxDifference(_originalData, _noiseData);
        
        Vector3 resultMse = AnalysisOperations.MeanSquareError(_originalData, _data);
        Vector3 resultPmse = AnalysisOperations.PeakMeanSquareError(_originalData, _data);
        Vector3 resultSnr = AnalysisOperations.SignalNoiseRatio(_originalData, _data);
        Vector3 resultPsnr = AnalysisOperations.PeakSignalNoiseRatio(_originalData, _data);
        Vector3 resultMd = AnalysisOperations.MaxDifference(_originalData, _data);

        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Mean Square Error", "R", "G", "B");
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Original - Noise", initialMse.X, initialMse.Y, initialMse.Z);
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}\n", "Original - Result", resultMse.X, resultMse.Y, resultMse.Z);
        
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Peak Mean Square Error", "R", "G", "B");
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Original - Noise", initialPmse.X, initialPmse.Y, initialPmse.Z);
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}\n", "Original - Result", resultPmse.X, resultPmse.Y, resultPmse.Z);

        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Signal Noise Ratio", "R", "G", "B");
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Original - Noise", initialSnr.X, initialSnr.Y, initialSnr.Z);
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}\n", "Original - Result", resultSnr.X, resultSnr.Y, resultSnr.Z);
        
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Peak Signal Noise Ratio", "R", "G", "B");
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Original - Noise", initialPsnr.X, initialPsnr.Y, initialPsnr.Z);
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}\n", "Original - Result", resultPsnr.X, resultPsnr.Y, resultPsnr.Z);
        
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Max Difference", "R", "G", "B");
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}", "Original - Noise", initialMd.X, initialMd.Y, initialMd.Z);
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12}\n", "Original - Result", resultMd.X, resultMd.Y, resultMd.Z);
        
        _bitmap.UnlockBits(_data);
    }
}