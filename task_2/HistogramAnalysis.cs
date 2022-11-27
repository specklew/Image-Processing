using System.Drawing.Imaging;
using image_processing_core;

namespace task_2;

public static class HistogramAnalysis
{
    public static unsafe void UniformFinalDensityProbabilityFunction(BitmapData data, int minBrightness, int maxBrightness)
    {
        int Calculate(int f, IReadOnlyList<int> bucket)
        {
            var sum = 0;
            for (var i = 0; i < f; i++)
            {
                sum += bucket[i];
            }
            
            return (int)(minBrightness + (double)(maxBrightness - minBrightness) / (data.Height * data.Width) * sum);
        }

        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;

        var histogram = new ImageHistogram(data);

        for (var y = 0; y < data.Height; y++)
        {
            byte* row = pt + y * data.Stride;
            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = row + x * bpp;
                var c = RGB.ToRGB(pixel);
                c = new RGB(Calculate(c.R, histogram.RedBucket), 
                            Calculate(c.G, histogram.GreenBucket), 
                            Calculate(c.B, histogram.BlueBucket));
                c.SaveToPixel(pixel);
            }
        }
    }

    public static double Mean(BitmapData data, Channel channel)
    {
        var histogram = new ImageHistogram(data);
        int[] bucket = histogram.Buckets[channel];

        double sum = 0;

        for (var i = 0; i < 256; i++)
        {
            sum += i * bucket[i];
        }

        return sum / (data.Width * data.Height);
    }

    public static double Variance(BitmapData data, Channel channel)
    {
        var histogram = new ImageHistogram(data);
        int[] bucket = histogram.Buckets[channel];
        double mean = Mean(data, channel);
        double sum = 0;

        for (var i = 0; i < 256; i++)
        {
            sum += Math.Pow(i - mean, 2) * bucket[i];
        }

        return sum / (data.Height * data.Width);
    }

    public static double StandardDeviation(BitmapData data, Channel channel)
    {
        return Math.Sqrt(Variance(data, channel));
    }

    public static double VariationCoefficientI(BitmapData data, Channel channel)
    {
        return StandardDeviation(data, channel) / Mean(data, channel);
    }

    public static double AsymmetryCoefficient(BitmapData data, Channel channel)
    {
        var histogram = new ImageHistogram(data);
        int[] bucket = histogram.Buckets[channel];
        double mean = Mean(data, channel);
        double sum = 0;

        for (var i = 0; i < 256; i++)
        {
            sum += Math.Pow(i - mean, 3) * bucket[i];
        }

        return 1 / Math.Pow(StandardDeviation(data, channel), 3) * 1 / (data.Height * data.Width) * sum;
    }

    public static double FlatteningCoefficient(BitmapData data, Channel channel)
    {
        var histogram = new ImageHistogram(data);
        int[] bucket = histogram.Buckets[channel];
        double mean = Mean(data, channel);
        double sum = 0;

        for (var i = 0; i < 256; i++)
        {
            sum += Math.Pow(i - mean, 4) * bucket[i];
        }

        return 1 / Math.Pow(StandardDeviation(data, channel), 4) * 1 / (data.Height * data.Width) * sum - 3;
    }

    public static double VariationCoefficientII(BitmapData data, Channel channel)
    {
        var histogram = new ImageHistogram(data);
        int[] bucket = histogram.Buckets[channel];
        double sum = 0;
        
        for (var i = 0; i < 256; i++)
        {
            sum += bucket[i] * bucket[i];
        }

        return 1 / Math.Pow(data.Height * data.Width, 2) * sum;
    }

    public static double InformationSourceEntropy(BitmapData data, Channel channel)
    {
        var histogram = new ImageHistogram(data);
        int[] bucket = histogram.Buckets[channel];
        double sum = 0;

        for (var i = 0; i < 256; i++)
        {
            if (bucket[i] > 0)
            {
                sum += bucket[i] * Math.Log2((double)bucket[i] / (data.Height * data.Width));
            }
        }

        return -1.0 / (data.Height * data.Width) * sum;
    }
}