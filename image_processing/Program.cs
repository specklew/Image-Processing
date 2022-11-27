using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using BenchmarkDotNet.Running;
using CommandLine;
using image_processing_core;
using task_1;
using task_2;

namespace image_processing;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(t =>
        {
            if (t.Benchmark)
            {
                Benchmark();
                return;
            }
            
            Bitmap bitmap = ImageIO.LoadImage($"{t.FilePath}\\noise.bmp");
            BitmapData data = ImageIO.LockPixels(bitmap);
            
            Elementary(t, data);
            Geometric(t, data, ref bitmap);
            Noise(t, data);
            Analysis(t, data);
            Histogram(t, data);
            Convolution(t, data, ref bitmap);

            bitmap.UnlockBits(data);

            // ReSharper disable once StringLiteralTypo
            ImageIO.SaveImage(bitmap, $"{t.FilePath}\\result.bmp");  
        });
    }

    private static void Benchmark()
    {
        var summary = BenchmarkRunner.Run<ConvolutionBenchmark>();
    }

    private static void Elementary(Options t, BitmapData data)
    {
        if (t.Brightness != 0) ElementaryOperations.ModifyBrightness(data, t.Brightness);
        if (t.Contrast != 0) ElementaryOperations.ModifyContrast(data, t.Contrast);
        if (t.Negative) ElementaryOperations.Negative(data);
    }

    private static void Geometric(Options t, BitmapData data, ref Bitmap bitmap)
    {
        if (t.VerticalFlip) GeometricOperations.VerticalFlip(data);
        if (t.HorizontalFlip) GeometricOperations.HorizontalFlip(data);
        if (t.DiagonalFlip) GeometricOperations.DiagonalFlip(data);
        if (t.Shrink != 0.0f) GeometricOperations.Resize(ref bitmap, data, 1 / t.Shrink);
        if (t.Enlarge != 0.0f) GeometricOperations.Resize(ref bitmap, data, t.Enlarge);
    }

    private static void Noise(Options t, BitmapData data)
    {
        if (t.HarmonicMeanFilter != Rectangle.Empty) NoiseRemoval.HarmonicFilter(data, t.HarmonicMeanFilter);
        if (t.MidpointFilter != Rectangle.Empty) NoiseRemoval.MidpointFilter(data, t.MidpointFilter);   
        if (t.ArithmeticMeanFilter != Rectangle.Empty) NoiseRemoval.ArithmeticMeanFilter(data, t.ArithmeticMeanFilter);
        if (t.MedianFilter != Rectangle.Empty) NoiseRemoval.MedianFilter(data, t.MedianFilter);
    }

    private static void Analysis(Options t, BitmapData data)
    {
        if (!t.MeanSquareError && !t.PeakMeanSquareError && !t.SignalNoiseRatio && !t.PeakSignalNoiseRatio &&
            !t.MaxDifference) return;
        
        Bitmap noise = ImageIO.LoadImage($"{t.FilePath}\\noise.bmp");
        Bitmap original = ImageIO.LoadImage($"{t.FilePath}\\original.bmp");
        
        BitmapData noiseData = ImageIO.LockPixels(noise);
        BitmapData originalData = ImageIO.LockPixels(original);
        
        if (t.MeanSquareError)
        {
            RGB64 error = AnalysisOperations.MeanSquareError(originalData, data);
            Console.WriteLine("MSE Original - Result: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
            error = AnalysisOperations.MeanSquareError(originalData, noiseData);
            Console.WriteLine("MSE Original - Noise: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
        }
        
        if (t.PeakMeanSquareError)
        {
            RGB64 error = AnalysisOperations.PeakMeanSquareError(data, originalData);
            Console.WriteLine("PMSE Original - Result: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
            error = AnalysisOperations.PeakMeanSquareError(originalData, noiseData);
            Console.WriteLine("PMSE Original - Noise: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
        }
        
        if (t.SignalNoiseRatio)
        {
            RGB64 error = AnalysisOperations.SignalNoiseRatio(data, originalData);
            Console.WriteLine("SNR Original - Result: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
            error = AnalysisOperations.SignalNoiseRatio(originalData, noiseData);
            Console.WriteLine("SNR Original - Noise: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
        }
        
        if (t.PeakSignalNoiseRatio)
        {
            RGB64 error = AnalysisOperations.PeakSignalNoiseRatio(data, originalData);
            Console.WriteLine("PSNR Original - Result: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
            error = AnalysisOperations.PeakSignalNoiseRatio(originalData, noiseData);
            Console.WriteLine("PSNR Original - Noise: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
        }
        
        if (t.MaxDifference)
        {
            RGB64 error = AnalysisOperations.MaxDifference(data, originalData);
            Console.WriteLine("Max difference Original - Result: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
            error = AnalysisOperations.MaxDifference(originalData, noiseData);
            Console.WriteLine("Max difference Original - Noise: R = " + error.R + ", G = " + error.G + ", B = " + error.B);
        }
        
        noise.UnlockBits(noiseData);
        original.UnlockBits(originalData);
    }

    private static void Histogram(Options t, BitmapData data)
    {
        if (t.Histogram)
        {
            var histogram = new ImageHistogram(data);
            ImageIO.SaveImage(histogram.GetHistogram(), $"{t.FilePath}\\histogram.bmp");
        }

        if (t.HUniform != Point.Empty)
        {
            HistogramAnalysis.UniformFinalDensityProbabilityFunction(data, t.HUniform.X, t.HUniform.Y);
        }

        var isFirstAnalysisOperation = true;

        void IfFirstCharacteristicWriteTableHeader()
        {
            if (!isFirstAnalysisOperation) return;
            WriteSpecificLine( "Histogram characteristic", "R", "G", "B");
            isFirstAnalysisOperation = false;
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

        if (t.Mean)
        {
            IfFirstCharacteristicWriteTableHeader();

            WriteCharacteristicLine("Mean",
                HistogramAnalysis.Mean(data, Channel.Red),
                HistogramAnalysis.Mean(data, Channel.Green),
                HistogramAnalysis.Mean(data, Channel.Blue));
        }

        if (t.Variance)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Variance", 
                HistogramAnalysis.Variance(data, Channel.Red), 
                HistogramAnalysis.Variance(data, Channel.Green), 
                HistogramAnalysis.Variance(data, Channel.Blue));
        }

        if (t.StandardDeviation)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Standard Deviation", 
                HistogramAnalysis.StandardDeviation(data, Channel.Red), 
                HistogramAnalysis.StandardDeviation(data, Channel.Green), 
                HistogramAnalysis.StandardDeviation(data, Channel.Blue));
        }
        
        if (t.VariationCoefficientI)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Variation Coefficient I", 
                HistogramAnalysis.VariationCoefficientI(data, Channel.Red), 
                HistogramAnalysis.VariationCoefficientI(data, Channel.Green), 
                HistogramAnalysis.VariationCoefficientI(data, Channel.Blue));
        }
        
        if (t.AsymmetryCoefficient)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Asymmetry Coefficient", 
                HistogramAnalysis.AsymmetryCoefficient(data, Channel.Red), 
                HistogramAnalysis.AsymmetryCoefficient(data, Channel.Green), 
                HistogramAnalysis.AsymmetryCoefficient(data, Channel.Blue));
        }
        
        if (t.FlatteningCoefficient)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Flattening Coefficient", 
                HistogramAnalysis.FlatteningCoefficient(data, Channel.Red), 
                HistogramAnalysis.FlatteningCoefficient(data, Channel.Green), 
                HistogramAnalysis.FlatteningCoefficient(data, Channel.Blue));
        }
        
        if (t.VariationCoefficientII)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Variation Coefficient II", 
                HistogramAnalysis.VariationCoefficientII(data, Channel.Red), 
                HistogramAnalysis.VariationCoefficientII(data, Channel.Green), 
                HistogramAnalysis.VariationCoefficientII(data, Channel.Blue));
        }
        
        if (t.InformationSourceEntropy)
        {
            IfFirstCharacteristicWriteTableHeader();
            
            WriteCharacteristicLine("Information Source Entropy", 
                HistogramAnalysis.InformationSourceEntropy(data, Channel.Red), 
                HistogramAnalysis.InformationSourceEntropy(data, Channel.Green), 
                HistogramAnalysis.InformationSourceEntropy(data, Channel.Blue));
        }
    }

    private static void Convolution(Options t, BitmapData data, ref Bitmap bitmap)
    {
        bitmap.UnlockBits(data);
        
        if (t.ExtractionOfDetails != null)
        {
            bitmap = ConvolutionOperations.ExtractDetails(bitmap, (int)t.ExtractionOfDetails);
        }
        if (t.OptimizedExtractionOfDetails)
        {
            bitmap = ConvolutionOperations.OptimizedEdgeDetection(bitmap);
        }

        if (t.RobertsOperatorI)
        {
            bitmap = NonLinearOperations.RobertsOpeator(bitmap);
        }

        ImageIO.LockPixels(bitmap);
    }
}