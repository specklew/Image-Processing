﻿using CommandLine;
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
}