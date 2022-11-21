using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using image_processing_core;
using GemBox.Spreadsheet;
using task_1;

namespace task_1_tests;

[TestFixture]
public class NoiseRemovalTests
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\lenna";
    private const string SavePath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\tests\\noise_removal";
    private const string NoiseName = "lenac_impulse3.bmp";
    
    private Bitmap _bitmap = null!;
    private Bitmap _noise = null!;
    private Bitmap _original = null!;
    
    private BitmapData _data = null!;
    private BitmapData _noiseData = null!;
    private BitmapData _originalData = null!;

    private Stopwatch _stopWatch = null!;
    private int _operationNumber = 1;

    [SetUp]
    public void Setup()
    {
        _bitmap = ImageIO.LoadImage($"{TestPath}\\{NoiseName}");
        _noise = ImageIO.LoadImage($"{TestPath}\\{NoiseName}");
        _original = ImageIO.LoadImage($"{TestPath}\\original.bmp");

        _data = ImageIO.LockPixels(_bitmap);
        _noiseData = ImageIO.LockPixels(_noise);
        _originalData = ImageIO.LockPixels(_original);
        
        _stopWatch = Stopwatch.StartNew();   
    }

    [Test]
    public void HarmonicMeanTest3X3()
    {
        NoiseRemoval.HarmonicFilter(_data, new Rectangle(0, 0, 3, 3));
    }
    
    [Test]
    public void MedianTest3X3()
    {
        NoiseRemoval.MedianFilter(_data, new Rectangle(0, 0, 3, 3));
    }
    
    [Test]
    public void HarmonicMeanTest5X5()
    {
        NoiseRemoval.HarmonicFilter(_data, new Rectangle(0, 0, 5, 5));
    }
    
    [Test]
    public void MedianTest5X5()
    {
        NoiseRemoval.MedianFilter(_data, new Rectangle(0, 0, 5, 5));
    }
    
    [Test]
    public void HarmonicMeanTest7X7()
    {
        NoiseRemoval.HarmonicFilter(_data, new Rectangle(0, 0, 7, 7));
    }
    
    [Test]
    public void MedianTest7X7()
    {
        NoiseRemoval.MedianFilter(_data, new Rectangle(0, 0, 7, 7));
    }

    [Test]
    public void HarmonicMeanTest9X9()
    {
        NoiseRemoval.HarmonicFilter(_data, new Rectangle(0, 0, 9, 9));
    }
    
    [Test]
    public void MedianTest9X9()
    {
        NoiseRemoval.MedianFilter(_data, new Rectangle(0, 0, 9, 9));
    }
    
    // [Test]
    // public void MedianAndHarmonicTest3X3()
    // {
    //     NoiseRemoval.HarmonicFilter(_data, new Rectangle(0,0,3,3));
    //     NoiseRemoval.MedianFilter(_data, new Rectangle(0, 0, 3, 3));
    // }
    //     
    // [Test]
    // public void GeometricMeanFilterTest()
    // {
    //     NoiseRemoval.GeometricMeanFilter(_data, new Rectangle(0,0,3,3));
    // }
    
    
    [TearDown] 
    public void TearDown()
    {
        _stopWatch.Stop();
        _operationNumber++;
        
        RGB64 initialMse = AnalysisOperations.MeanSquareError(_originalData, _noiseData);
        RGB64 initialPmse = AnalysisOperations.PeakMeanSquareError(_originalData, _noiseData);
        RGB64 initialSnr = AnalysisOperations.SignalNoiseRatio(_originalData, _noiseData);
        RGB64 initialPsnr = AnalysisOperations.PeakSignalNoiseRatio(_originalData, _noiseData);
        RGB64 initialMd = AnalysisOperations.MaxDifference(_originalData, _noiseData);
        
        RGB64 resultMse = AnalysisOperations.MeanSquareError(_originalData, _data);
        RGB64 resultPmse = AnalysisOperations.PeakMeanSquareError(_originalData, _data);
        RGB64 resultSnr = AnalysisOperations.SignalNoiseRatio(_originalData, _data);
        RGB64 resultPsnr = AnalysisOperations.PeakSignalNoiseRatio(_originalData, _data);
        RGB64 resultMd = AnalysisOperations.MaxDifference(_originalData, _data);

        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12} | {4,-12}", "Mean Square Error", "R", "G", "B", "Mean");
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}", "Original - Noise", initialMse.R, initialMse.G, initialMse.B, initialMse.Mean());
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}\n", "Original - Result", resultMse.R, resultMse.G, resultMse.B, resultMse.Mean());
        
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12} | {4,-12}", "Peak Mean Square Error", "R", "G", "B", "Mean");
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}", "Original - Noise", initialPmse.R, initialPmse.G, initialPmse.B, initialPmse.Mean());
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}\n", "Original - Result", resultPmse.R, resultPmse.G, resultPmse.B, resultPmse.Mean());

        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12} | {4,-12}", "Signal Noise Ratio", "R", "G", "B", "Mean");
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}", "Original - Noise", initialSnr.R, initialSnr.G, initialSnr.B, initialSnr.Mean());
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}\n", "Original - Result", resultSnr.R, resultSnr.G, resultSnr.B, resultSnr.Mean());
        
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12} | {4,-12}", "Peak Signal Noise Ratio", "R", "G", "B", "Mean");
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}", "Original - Noise", initialPsnr.R, initialPsnr.G, initialPsnr.B, initialPsnr.Mean());
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}\n", "Original - Result", resultPsnr.R, resultPsnr.G, resultPsnr.B, resultPsnr.Mean());
        
        Console.WriteLine("{0,-25} | {1,-12} | {2,-12} | {3,-12} | {4,-12}", "Max Difference", "R", "G", "B", "Mean");
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}", "Original - Noise", initialMd.R, initialMd.G, initialMd.B, initialMd.Mean());
        Console.WriteLine("{0,-25} | {1,-12:F3} | {2,-12:F3} | {3,-12:F3} | {4,-12:F3}\n", "Original - Result", resultMd.R, resultMd.G, resultMd.B, resultMd.Mean());
        
        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

        ExcelFile workbook = ExcelFile.Load(SavePath + "\\AnalysisResults.xlsx");
        ExcelWorksheet worksheet = workbook.Worksheets.Count <= 0 ? workbook.Worksheets.Add("sheet") : workbook.Worksheets[0];

        if (_operationNumber <= 2)
        {
            worksheet.Cells[0, 0].Value = "Filter";
            worksheet.Cells[0, 1].Value = "Input";
            worksheet.Cells[0, 2].Value = "Resolution";
            worksheet.Cells[0, 3].Value = "Time[ms]";
            worksheet.Cells[0, 4].Value = "MSE";
            worksheet.Cells[0, 5].Value = "PMSE";
            worksheet.Cells[0, 6].Value = "SNR";
            worksheet.Cells[0, 7].Value = "PSNR";
            worksheet.Cells[0, 8].Value = "MD";
            
            worksheet.Cells[1, 0].Value = "No filter";
            worksheet.Cells[1, 1].Value = NoiseName;
            worksheet.Cells[1, 2].Value = $"{_bitmap.Width}x{_bitmap.Height}";
            worksheet.Cells[1, 3].Value = "-";
            worksheet.Cells[1, 4].Value = initialMse.Mean().ToString("F3");
            worksheet.Cells[1, 5].Value = initialPmse.Mean().ToString("F3");
            worksheet.Cells[1, 6].Value = initialSnr.Mean().ToString("F3");
            worksheet.Cells[1, 7].Value = initialPsnr.Mean().ToString("F3");
            worksheet.Cells[1, 8].Value = initialMd.Mean().ToString("F3");
        }
        
        worksheet.Cells[_operationNumber, 0].Value = TestContext.CurrentContext.Test.Name;
        worksheet.Cells[_operationNumber, 1].Value = NoiseName;
        worksheet.Cells[_operationNumber, 2].Value = $"{_bitmap.Width}x{_bitmap.Height}";
        worksheet.Cells[_operationNumber, 3].Value = _stopWatch.ElapsedMilliseconds;
        worksheet.Cells[_operationNumber, 4].Value = resultMse.Mean().ToString("F3");
        worksheet.Cells[_operationNumber, 5].Value = resultPmse.Mean().ToString("F3");
        worksheet.Cells[_operationNumber, 6].Value = resultSnr.Mean().ToString("F3");
        worksheet.Cells[_operationNumber, 7].Value = resultPsnr.Mean().ToString("F3");
        worksheet.Cells[_operationNumber, 8].Value = resultMd.Mean().ToString("F3");

        workbook.Save(SavePath + "\\AnalysisResults.xlsx");
        
        _bitmap.UnlockBits(_data);
        Console.WriteLine($"Saving current operation under: {SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp\n");
        ImageIO.SaveImage(_bitmap, $"{SavePath}\\{TestContext.CurrentContext.Test.Name}.bmp");
    }
}