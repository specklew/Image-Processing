using System.ComponentModel;
using System.Drawing;
using CommandLine;
using image_processing_core;

// ReSharper disable All
#pragma warning disable CS8618

namespace image_processing;

public class Options
{
    [Value(0)]
    public string FilePath { get; set; }

    [Option("benchmark")]
    public bool Benchmark { get; set; }
    
    [Option("brightness")]
    public byte Brightness { get; set; }

    [Option("contrast")]
    public byte Contrast { get; set; }
    
    [Option("negative")]
    public bool Negative { get; set; }
    
    [Option("vflip")]
    public bool VerticalFlip { get; set; }
    
    [Option("hflip")]
    public bool HorizontalFlip { get; set; }
    
    [Option("dflip")]
    public bool DiagonalFlip { get; set; }
    
    [Option("shrink")]
    public float Shrink { get; set; }
    
    [Option("enlarge")]
    public float Enlarge { get; set; }
    
    [Option("hmean")]
    public Rectangle HarmonicMeanFilter { get; set; }
    
    [Option("amean")]
    public Rectangle ArithmeticMeanFilter { get; set; }
    
    [Option("mid")]
    public Rectangle MidpointFilter { get; set; }
    
    [Option("median")]
    public Rectangle MedianFilter { get; set; }
    
    [Option("mse")]
    public bool MeanSquareError { get; set; }
    
    [Option("pmse")]
    public bool PeakMeanSquareError { get; set; }
    
    [Option("snr")]
    public bool SignalNoiseRatio { get; set; }
    
    [Option("psnr")]
    public bool PeakSignalNoiseRatio { get; set; }
    
    [Option("md")]
    public bool MaxDifference { get; set; }
    
    [Option("histogram")]
    public bool Histogram { get; set; }
    
    [Option("huniform")]
    public Point HUniform { get; set; }
    
    [Option("cmean")]
    public bool Mean { get; set; }
    
    [Option("cvariance")]
    public bool Variance { get; set; }
    
    [Option("cstdev")]
    public bool StandardDeviation { get; set; }
    
    [Option("cvarcoi")]
    public bool VariationCoefficientI { get; set; }
    
    [Option("casyco")]
    public bool AsymmetryCoefficient { get; set; }
    
    [Option("cflaco")]
    public bool FlatteningCoefficient { get; set; }
    
    [Option("cvarcoii")]
    public bool VariationCoefficientII { get; set; }
    
    [Option("centropy")]
    public bool InformationSourceEntropy { get; set; }
    
    [Option("slaplace")]
    public int? ExtractionOfDetails { get; set; }
    
    [Option("solaplace")]
    public bool OptimizedExtractionOfDetails { get; set; }
    
    [Option("orobertsi")]
    public bool RobertsOperatorI { get; set; }
    
    [Option("osobel")]
    public bool SobelOperator { get; set; }
    
    [Option("ouolis")]
    public bool UolisOperator { get; set; }

    [Option("erode")]
    public string Erode { get; set; }

    [Option("dilate")]
    public string Dilate { get; set; }

    [Option("opening")]
    public string Opening { get; set; }

    [Option("closing")]
    public string Closing { get; set; }

    [Option("hmt")]
    public string HitOrMiss { get; set; }

    [Option("m3")]
    public IEnumerable<string> M3 { get; set; }
}