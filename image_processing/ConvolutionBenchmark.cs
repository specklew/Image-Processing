using System.Drawing;
using System.Drawing.Imaging;
using BenchmarkDotNet.Attributes;
using task_2;

namespace image_processing;


[SimpleJob(launchCount: 3, warmupCount: 10, targetCount: 30)]
[MemoryDiagnoser]
public class ConvolutionBenchmark
{
    private const string TestPath = "C:\\Studia\\2022_Winter\\Image Processing\\Labs\\lenna";
    private Bitmap _bitmap;
    private Bitmap newBitmap;
    private BitmapData data;
    private BitmapData newData;
    
    private static readonly int[,] Laplace1 =
    {
        { 0,-1, 0},
        {-1, 4,-1},
        { 0,-1, 0}
    };

    [IterationSetup]
    public void Setup()
    {
        _bitmap = ImageIO.LoadImage($"{TestPath}\\original.bmp");
        data = ImageIO.LockPixels(_bitmap);
        newBitmap = ImageIO.LoadImage($"{TestPath}\\original.bmp");
        newData = ImageIO.LockPixels(newBitmap);
    }
    
    [Benchmark]
    public void NormalConvolution()
    {
        ConvolutionOperations.PerformConvolution(data, newData, Laplace1, 1f);
    }   
    
    [Benchmark]
    public void OptimizedConvolution()
    {
        ConvolutionOperations.ApplyOptimizedEdgeDetection(data, newData);
    }   
    
    [Benchmark]
    public void AllocationHeavyConvolution()
    {
        ConvolutionOperations.ApplyAllocationHeavyOptimizedEdgeDetection(data, newData);
    }   
    
    [IterationCleanup]
    public void GlobalCleanup()
    {
        _bitmap.UnlockBits(data);
        newBitmap.UnlockBits(newData);
    }
}