using System.Drawing;
using CommandLine;
// ReSharper disable All
#pragma warning disable CS8618

namespace image_processing;

public class Options
{
    [Value(0)]
    public string FilePath { get; set; }

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
}