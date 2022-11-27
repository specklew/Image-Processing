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

    private static FileInfo _currentFileInfo = null!;

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

            _currentFileInfo = fileInfo;
            
            Bitmap bitmap = ImageIO.LoadImage(fileInfo.FullName);
            BitmapData data = ImageIO.LockPixels(bitmap);
            
            AnalyseFile(data, fileInfo, worksheet);
            
            bitmap.UnlockBits(data);
        }
        
        workbook.Save(SavePath + "\\AnalysisResults.xlsx");
    }

    private static void AnalyseFile(BitmapData data, FileInfo fileInfo, ExcelWorksheet worksheet)
    {
        var histogram = new ImageHistogram(data);
        ImageIO.SaveImage(histogram.GetHistogram(), $"{SavePath}\\{fileInfo.Name}_histogram.bmp");

        WriteCharacteristicsForImage(worksheet, data);
    }

    private static void WriteCharacteristicsForImage(ExcelWorksheet worksheet, BitmapData data)
    {
        WriteFirstLine(worksheet);
        WriteLineForChannel(worksheet, data, Channel.Red);
        WriteLineForChannel(worksheet, data, Channel.Green);
        WriteLineForChannel(worksheet, data, Channel.Blue);
    }

    private static void WriteLineForChannel(ExcelWorksheet worksheet, BitmapData data, Channel channel)
    {
        int lineNumber = GetFirstBlankRow(worksheet);
        worksheet.Cells[lineNumber, 0].SetValue(channel.ToString());
        worksheet.Cells[lineNumber, 1].SetValue(HistogramAnalysis.Mean(data, channel));
        worksheet.Cells[lineNumber, 2].SetValue(HistogramAnalysis.Variance(data, channel));
        worksheet.Cells[lineNumber, 3].SetValue(HistogramAnalysis.StandardDeviation(data, channel));
        worksheet.Cells[lineNumber, 4].SetValue(HistogramAnalysis.VariationCoefficientI(data, channel));
        worksheet.Cells[lineNumber, 5].SetValue(HistogramAnalysis.AsymmetryCoefficient(data, channel));
        worksheet.Cells[lineNumber, 6].SetValue(HistogramAnalysis.FlatteningCoefficient(data, channel));
        worksheet.Cells[lineNumber, 7].SetValue(HistogramAnalysis.VariationCoefficientII(data, channel));
        worksheet.Cells[lineNumber, 8].SetValue(HistogramAnalysis.InformationSourceEntropy(data, channel));
    }

    private static void WriteFirstLine(ExcelWorksheet worksheet)
    {
        int lineNumber = GetFirstBlankRow(worksheet) + 1;
        worksheet.Cells[lineNumber, 0].SetValue(_currentFileInfo.Name);
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