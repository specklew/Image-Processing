using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using CommandLine;
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
            Bitmap bitmap = ImageIO.LoadImage($"{t.FilePath}\\noise.bmp");
            BitmapData data = ImageIO.LockPixels(bitmap);
            
            Elementary(t, data);
            Geometric(t, data, ref bitmap);
            Noise(t, data);
            Analysis(t, data);
            HistogramCalculation(t, data);

            bitmap.UnlockBits(data);
            
            // ReSharper disable once StringLiteralTypo
            ImageIO.SaveImage(bitmap, $"{t.FilePath}\\result.bmp");  
        });
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
            Vector3 error = AnalysisOperations.MeanSquareError(originalData, data);
            Console.WriteLine("MSE Original - Result: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            error = AnalysisOperations.MeanSquareError(originalData, noiseData);
            Console.WriteLine("MSE Original - Noise: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
        }
        
        if (t.PeakMeanSquareError)
        {
            Vector3 error = AnalysisOperations.PeakMeanSquareError(data, originalData);
            Console.WriteLine("PMSE Original - Result: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            error = AnalysisOperations.PeakMeanSquareError(originalData, noiseData);
            Console.WriteLine("PMSE Original - Noise: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
        }
        
        if (t.SignalNoiseRatio)
        {
            Vector3 error = AnalysisOperations.SignalNoiseRatio(data, originalData);
            Console.WriteLine("SNR Original - Result: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            error = AnalysisOperations.SignalNoiseRatio(originalData, noiseData);
            Console.WriteLine("SNR Original - Noise: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
        }
        
        if (t.PeakSignalNoiseRatio)
        {
            Vector3 error = AnalysisOperations.PeakSignalNoiseRatio(data, originalData);
            Console.WriteLine("PSNR Original - Result: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            error = AnalysisOperations.PeakSignalNoiseRatio(originalData, noiseData);
            Console.WriteLine("PSNR Original - Noise: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
        }
        
        if (t.MaxDifference)
        {
            Vector3 error = AnalysisOperations.MaxDifference(data, originalData);
            Console.WriteLine("Max difference Original - Result: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            error = AnalysisOperations.MaxDifference(originalData, noiseData);
            Console.WriteLine("Max difference Original - Noise: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
        }
        
        noise.UnlockBits(noiseData);
        original.UnlockBits(originalData);
    }
    
    private static void HistogramCalculation(Options t, BitmapData data)
    {
        if (t.Histogram)
        {
            var histogram = new ImageHistogram(data);
            ImageIO.SaveImage(histogram.GetHistogram(), $"{t.FilePath}\\histogram.bmp");
        }
    }

}