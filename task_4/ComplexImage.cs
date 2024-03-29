﻿using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using image_processing_core;

namespace task_4;

public partial class ComplexImage
{
    private Complex[,] _data;
    private int _width;
    private int _height;
    private bool _fourierTransformed = false;

    public int Width => _width;
    public int Height => _height;
    public bool FourierTransformed => _fourierTransformed;
    public Complex[,] Data => _data;

    protected ComplexImage(int width, int height)
    {
        _width = width;
        _height = height;
        
        _data = new Complex[width, height];
        _fourierTransformed = false;
    }

    public static ComplexImage FromBitmap(Bitmap image)
    {
        BitmapData data = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadOnly, image.PixelFormat);

        ComplexImage complexImage = FromBitmap(data);
        
        image.UnlockBits(data);

        return complexImage;
    }

    public static ComplexImage FromBitmap(BitmapData imageData)
    {
        var complexImage = new ComplexImage( imageData.Width, imageData.Height );
        
        IntPtr ptr = imageData.Scan0;
        int bpp = imageData.Stride / imageData.Width;
        for (var y = 0; y < imageData.Height; y++)
        {
            IntPtr row = ptr + y * imageData.Stride;
            for (var x = 0; x < imageData.Width; x++)
            {
                IntPtr pixel = row + bpp * x;
                byte value = Marshal.ReadByte(pixel);
                complexImage._data[x,y] = (double)value;
            }
        }

        return complexImage;
    }
    
    public unsafe Bitmap ToBitmap()
    {
        if (_fourierTransformed)
        {
            return InvertFFT();
        }
        
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);

        BitmapData bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)), 
            ImageLockMode.ReadWrite, image.PixelFormat);

        var ptr = (byte*)bits.Scan0;
        int bpp = bits.Stride / _width;

        for (var y = 0; y < _height; y++)
        {
            byte* row = ptr + y * bits.Stride;
            for(var x = 0; x < _width; x++)
            {
                byte* pixel = row + x * bpp;
                var rgb = new RGB64(_data[x,y].Magnitude);
                rgb.SaveToPixel(pixel);
            }
        }
        image.UnlockBits(bits);

        return image;
    }
}