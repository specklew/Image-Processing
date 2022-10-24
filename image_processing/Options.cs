using System.Drawing;
using CommandLine;
// ReSharper disable All
#pragma warning disable CS8618

namespace image_processing;

public class Options
{
    [Value(0)]
    public string FilePath { get; set; }

    [Option('b', "brightness")]
    public byte Brightness { get; set; }

    [Option('c', "contrast")]
    public byte Contrast { get; set; }
    
    [Option('n', "negative")]
    public bool Negative { get; set; }
    
    [Option('v', "vflip")]
    public bool VerticalFlip { get; set; }
    
    [Option('h', "hflip")]
    public bool HorizontalFlip { get; set; }
    
    [Option('d', "dflip")]
    public bool DiagonalFlip { get; set; }
    
    [Option('s', "shrink")]
    public float Shrink { get; set; }
    
    [Option('e', "enlarge")]
    public float Enlarge { get; set; }
    
    [Option('h', "hmean")]
    public Rectangle HarmonicMeanFilter { get; set; }
    
    [Option('a', "amean")]
    public Rectangle ArithmeticMeanFilter { get; set; }
    
    [Option('m', "mid")]
    public Rectangle MidpointFilter { get; set; }
    
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
}