using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using image_processing_core;

namespace task_4;

public class ComplexImage
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
        
        _data = new Complex[height, width];
        _fourierTransformed = false;
    }

    public static ComplexImage FromBitmap(Bitmap image)
    {
        BitmapData data = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadOnly, image.PixelFormat);

        ComplexImage complexImage = FromBitmap(data);

        return complexImage;
    }

    public static ComplexImage FromBitmap(BitmapData imageData)
    {
        var complexImage = new ComplexImage( imageData.Width, imageData.Height );
        Complex[,] data = complexImage._data;

        IntPtr ptr = imageData.Scan0;
        int bpp = imageData.Stride / imageData.Width;
        for (var y = 0; y < imageData.Height; y++)
        {
            IntPtr row = ptr + y * imageData.Stride;
            for (var x = 0; x < imageData.Width; x++)
            {
                IntPtr pixel = row + bpp * x;
                byte value = Marshal.ReadByte(pixel);
                data[x, y] = (double)value / 255;
            }
        }

        return complexImage;
    }
    
    public unsafe Bitmap ToBitmap()
    {
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);

        BitmapData bits = image.LockBits(
            new Rectangle(0, 0, _width, _height), 
            ImageLockMode.ReadWrite, image.PixelFormat);
        
        double scale = _fourierTransformed ? Math.Sqrt( _width * _height ) : 1;

        var ptr = (byte*)bits.Scan0;
        int bpp = bits.Stride / bits.Width;

        for (var y = 0; y < _height; y++)
        {
            byte* row = ptr + y * bits.Stride;
            for(var x = 0; x < _width; x++)
            {
                byte* pixel = row + x * bpp;
                var rgb = new RGB64(_data[x, y].Magnitude * scale * 255 );
                rgb.SaveToPixel(pixel);
            }
        }
        image.UnlockBits(bits);

        return image;
    }

    public void DiscreteFourierTransform()
    {
        if (_fourierTransformed) return;

        Complex[,] temp = new Complex[_width,_height];

        var iteration = 0;
        
        Parallel.For(0, _width, x =>
        {
            iteration++;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var y = 0; y < _height; y++)
            {
                
                for (var i = 0; i < _width; i++)
                {
                    for (var j = 0; j < _height; j++)
                    {
                        double angle = 2 * Math.PI * (x * i / _width + y * j / _height);
                        Complex c = new Complex(Math.Cos(angle), -Math.Sin(angle));
                        temp[x, y] += _data[i, j] * c;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine("Iteration ~= " + iteration + " elapsed = " + stopwatch.ElapsedMilliseconds + " ms");
        });

        _data = temp;
        _fourierTransformed = true;
    }
}