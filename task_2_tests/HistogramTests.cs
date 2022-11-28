using System.Drawing;
using System.Drawing.Imaging;
using GemBox.Spreadsheet;
using image_processing;
using image_processing_core;
using task_2;

namespace task_2_tests;

[TestFixture]
public class HistogramTests
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\test_images";
    private const string SavePath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\tests\\histogram";

    private static string _currentName = null!;

    [Test]
    public void AnalyseEachBitmapInFolder()
    {
        DirectoryInfo dir = new DirectoryInfo(TestPath);
        FileInfo[] files = dir.GetFiles();

        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        
        ExcelFile workbook = new ExcelFile();
        ExcelWorksheet worksheet = workbook.Worksheets.Add("imagesResults");
        worksheet.SetDefaultColumnWidth( 180,LengthUnit.Pixel);

        foreach (FileInfo fileInfo in files)
        {
            if (fileInfo.Extension != ".bmp") continue;

            _currentName = fileInfo.Name;
            if (!Directory.Exists(SavePath + "\\" + _currentName))
            {
                Directory.CreateDirectory(SavePath + "\\" + _currentName);
            }
            
            Bitmap bitmap = ImageIO.LoadImage(fileInfo.FullName);

            AnalyseFile(bitmap, fileInfo, worksheet);
        }
        
        workbook.Save(SavePath + "\\AnalysisResults.xlsx");
    }

    private static void AnalyseFile(Bitmap bitmap, FileInfo fileInfo, ExcelWorksheet worksheet)
    {
        var processedBitmap0255 = bitmap.Clone() as Bitmap;
        var processedBitmap100200 = bitmap.Clone() as Bitmap;
        
        ImageIO.SaveImage(bitmap, $"{SavePath}\\{_currentName}\\unprocessed.bmp");
        
        BitmapData data = ImageIO.LockPixels(bitmap);
        var histogram = new ImageHistogram(data);
        bitmap.UnlockBits(data);
        
        ImageIO.SaveImage(histogram.GetHistogram(), $"{SavePath}\\{_currentName}\\unprocessed_histogram.bmp");

        WriteCharacteristicsForImage(worksheet, bitmap);
        
        ProcessFile(processedBitmap0255, 0, 255);

        WriteCharacteristicsForImage(worksheet, processedBitmap0255);
        
        ProcessFile(processedBitmap100200, 0, 180);

        WriteCharacteristicsForImage(worksheet, processedBitmap100200);
    }

    private static void ProcessFile(Bitmap bitmap, int minBrightness, int maxBrightness)
    {
        BitmapData data = ImageIO.LockPixels(bitmap);
        
        HistogramAnalysis.UniformFinalDensityProbabilityFunction(data, minBrightness, maxBrightness);
        var histogram = new ImageHistogram(data);
        ImageIO.SaveImage(histogram.GetHistogram(), $"{SavePath}\\{_currentName}\\processed_{minBrightness}_{maxBrightness}_histogram.bmp");
        
        bitmap.UnlockBits(data);
        ImageIO.SaveImage(bitmap, $"{SavePath}\\{_currentName}\\processed_{minBrightness}_{maxBrightness}.bmp");
    }

    private static void WriteCharacteristicsForImage(ExcelWorksheet worksheet, Bitmap bitmap)
    {
        BitmapData data = ImageIO.LockPixels(bitmap);
        WriteFirstLine(worksheet);
        WriteLineForChannel(worksheet, data, Channel.Red);
        WriteLineForChannel(worksheet, data, Channel.Green);
        WriteLineForChannel(worksheet, data, Channel.Blue);
        bitmap.UnlockBits(data);
    }

    private static void WriteLineForChannel(ExcelWorksheet worksheet, BitmapData data, Channel channel)
    {
        int lineNumber = GetFirstBlankRow(worksheet);
        worksheet.Cells[lineNumber, 0].SetValue(channel.ToString());
        worksheet.Cells[lineNumber, 1].SetValue(HistogramAnalysis.Mean(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 2].SetValue(HistogramAnalysis.Variance(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 3].SetValue(HistogramAnalysis.StandardDeviation(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 4].SetValue(HistogramAnalysis.VariationCoefficientI(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 5].SetValue(HistogramAnalysis.AsymmetryCoefficient(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 6].SetValue(HistogramAnalysis.FlatteningCoefficient(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 7].SetValue(HistogramAnalysis.VariationCoefficientII(data, channel).ToString("F3"));
        worksheet.Cells[lineNumber, 8].SetValue(HistogramAnalysis.InformationSourceEntropy(data, channel).ToString("F3"));
    }

    private static void WriteFirstLine(ExcelWorksheet worksheet)
    {
        int lineNumber = GetFirstBlankRow(worksheet) + 1;
        worksheet.Cells[lineNumber, 0].SetValue(_currentName);
        worksheet.Cells[lineNumber, 1].SetValue("Mean");
        worksheet.Cells[lineNumber, 2].SetValue("Variance");
        worksheet.Cells[lineNumber, 3].SetValue("Standard Deviation");
        worksheet.Cells[lineNumber, 4].SetValue("Variation Coefficient I");
        worksheet.Cells[lineNumber, 5].SetValue("Asymmetry Coefficient");
        worksheet.Cells[lineNumber, 6].SetValue("Flattening Coefficient");
        worksheet.Cells[lineNumber, 7].SetValue("Variation Coefficient II");
        worksheet.Cells[lineNumber, 8].SetValue("Information Source Entropy");
    }

    private static int GetFirstBlankRow(ExcelWorksheet worksheet)
    {
        var number = worksheet.Rows.Count;
        return number;
    }
}