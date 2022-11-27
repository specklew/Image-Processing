using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using image_processing_core;
using task_2;

namespace task_2_tests;

[TestFixture]
public class HistogramAnalysisTests
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

    void WriteCharacteristicLine(string characteristic, double red, double green, double blue)
    {
        var rs = red.ToString("F5");
        var gs = green.ToString("F5");
        var bs = blue.ToString("F5");
        WriteSpecificLine(characteristic, rs, gs, bs);
    }

    void WriteSpecificLine(string first, string second, string third, string fourth)
    {
        Console.WriteLine("{0,-26} | {1,-20} | {2,-20} | {3,-20}", first, second, third, fourth);
    }
    
    [Test]
    public void CharacteristicsTest()
    {
        WriteSpecificLine( "Histogram characteristic", "R", "G", "B");

        WriteCharacteristicLine("Mean",
            HistogramAnalysis.Mean(_data, Channel.Red),
            HistogramAnalysis.Mean(_data, Channel.Green),
            HistogramAnalysis.Mean(_data, Channel.Blue));

        WriteCharacteristicLine("Variance", 
            HistogramAnalysis.Variance(_data, Channel.Red), 
            HistogramAnalysis.Variance(_data, Channel.Green), 
            HistogramAnalysis.Variance(_data, Channel.Blue));
        
        WriteCharacteristicLine("Standard Deviation", 
            HistogramAnalysis.StandardDeviation(_data, Channel.Red), 
            HistogramAnalysis.StandardDeviation(_data, Channel.Green), 
            HistogramAnalysis.StandardDeviation(_data, Channel.Blue));

        WriteCharacteristicLine("Variation Coefficient I", 
            HistogramAnalysis.VariationCoefficientI(_data, Channel.Red), 
            HistogramAnalysis.VariationCoefficientI(_data, Channel.Green), 
            HistogramAnalysis.VariationCoefficientI(_data, Channel.Blue));

        WriteCharacteristicLine("Asymmetry Coefficient", 
            HistogramAnalysis.AsymmetryCoefficient(_data, Channel.Red), 
            HistogramAnalysis.AsymmetryCoefficient(_data, Channel.Green), 
            HistogramAnalysis.AsymmetryCoefficient(_data, Channel.Blue));

        WriteCharacteristicLine("Flattening Coefficient", 
            HistogramAnalysis.FlatteningCoefficient(_data, Channel.Red), 
            HistogramAnalysis.FlatteningCoefficient(_data, Channel.Green), 
            HistogramAnalysis.FlatteningCoefficient(_data, Channel.Blue));

        WriteCharacteristicLine("Variation Coefficient II", 
            HistogramAnalysis.VariationCoefficientII(_data, Channel.Red), 
            HistogramAnalysis.VariationCoefficientII(_data, Channel.Green), 
            HistogramAnalysis.VariationCoefficientII(_data, Channel.Blue));

        WriteCharacteristicLine("Information Source Entropy", 
            HistogramAnalysis.InformationSourceEntropy(_data, Channel.Red), 
            HistogramAnalysis.InformationSourceEntropy(_data, Channel.Green), 
            HistogramAnalysis.InformationSourceEntropy(_data, Channel.Blue));
        }

    [TearDown]
    public void TearDown()
    {
        _bitmap.UnlockBits(_data);
    }
}