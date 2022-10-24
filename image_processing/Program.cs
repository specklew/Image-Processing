using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using CommandLine;
using task_1;

namespace image_processing;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(t =>
        {
            //If parsing is successful.
            Bitmap bitmap = ImageIO.LoadImage(t.FilePath);
            Bitmap original = ImageIO.LoadImage(t.FilePath);
            BitmapData data = ImageIO.LockPixels(bitmap);

            if (t.Brightness != 0) ElementaryOperations.ModifyBrightness(data, t.Brightness);
            if (t.Contrast != 0) ElementaryOperations.ModifyContrast(data, t.Contrast);
            if (t.Negative) ElementaryOperations.Negative(data);
            if (t.VerticalFlip) GeometricOperations.VerticalFlip(data);
            if (t.HorizontalFlip) GeometricOperations.HorizontalFlip(data);
            if (t.DiagonalFlip) GeometricOperations.DiagonalFlip(data);
            if (t.Shrink != 0.0f) GeometricOperations.DiagonalFlip(data);
            if (t.Enlarge != 0.0f) GeometricOperations.DiagonalFlip(data);
            if (t.HarmonicMeanFilter != Rectangle.Empty) NoiseRemoval.HarmonicFilter(data, t.HarmonicMeanFilter);
            if (t.MidpointFilter != Rectangle.Empty) NoiseRemoval.MidpointFilter(data, t.MidpointFilter);   
            if (t.ArithmeticMeanFilter != Rectangle.Empty) NoiseRemoval.ArithmeticMeanFilter(data, t.ArithmeticMeanFilter);
            
            // Analysis:
            
            BitmapData originalData = ImageIO.LockPixels(original);
            
            if (t.MeanSquareError)
            {
                Vector3 error = AnalysisOperations.MeanSquareError(data, originalData);
                Console.WriteLine("Mean square error: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            }
            
            if (t.PeakMeanSquareError)
            {
                Vector3 error = AnalysisOperations.PeakMeanSquareError(data, originalData);
                Console.WriteLine("Peak mean square error: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            }
            
            if (t.SignalNoiseRatio)
            {
                Vector3 error = AnalysisOperations.SignalNoiseRatio(data, originalData);
                Console.WriteLine("Signal noise ratio: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            }
            
            if (t.PeakSignalNoiseRatio)
            {
                Vector3 error = AnalysisOperations.PeakSignalNoiseRatio(data, originalData);
                Console.WriteLine("Peak signal noise ratio: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            }
            
            if (t.MaxDifference)
            {
                Vector3 error = AnalysisOperations.MaxDifference(data, originalData);
                Console.WriteLine("Max difference: R = " + error.X + ", G = " + error.Y + ", B = " + error.Z);
            }
            
            original.UnlockBits(originalData);
            
            // Save to file:
            
            bitmap.UnlockBits(data);
            ImageIO.SaveImage(bitmap, "C:\\Studia\\2022_Winter\\Image Processing\\Tinkering\\result.bmp");  
        });
    }
}