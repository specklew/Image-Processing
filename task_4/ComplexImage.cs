using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using image_processing_core;

namespace Task4;

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
        var data = image.LockBits(new Rectangle(Point.Empty, image.Size), ImageLockMode.ReadOnly, image.PixelFormat);

        var complexImage = FromBitmap(data);

        return complexImage;
    }

    public static ComplexImage FromBitmap(BitmapData imageData)
    {
        var complexImage = new ComplexImage( imageData.Width, imageData.Height );
        var data = complexImage._data;

        var ptr = imageData.Scan0;
        var bpp = imageData.Stride / imageData.Width;
        for (var y = 0; y < imageData.Height; y++)
        {
            var row = ptr + y * imageData.Stride;
            for (var x = 0; x < imageData.Width; x++)
            {
                var pixel = row + bpp * x;
                var value = Marshal.ReadByte(pixel);
                data[x,y] = (double)value / 255;
            }
        }

        return complexImage;
    }
    
    public unsafe Bitmap ToBitmap()
    {
        if (_fourierTransformed)
        {
            return InverseDiscreteFourierTransform();
        }
        
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);

        var bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)), 
            ImageLockMode.ReadWrite, image.PixelFormat);

        var ptr = (byte*)bits.Scan0;
        var bpp = bits.Stride / _width;

        for (var y = 0; y < _height; y++)
        {
            var row = ptr + y * bits.Stride;
            for(var x = 0; x < _width; x++)
            {
                var pixel = row + x * bpp;
                var rgb = new RGB64(_data[x,y].Magnitude * 255);
                rgb.SaveToPixel(pixel);
            }
        }
        image.UnlockBits(bits);

        return image;
    }

    public unsafe Bitmap InverseDiscreteFourierTransform()
    {
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
        var bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)), 
            ImageLockMode.ReadWrite, image.PixelFormat);
        
        var temp = new Complex[_width, _height];

        for (var y = 0; y < _height; y++)
        {
            for (var k = 0; k < _width; k++)
            {
                var sum = Complex.Zero;
                for (var n = 0; n < _width; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _width),
                        Math.Sin(2 * Math.PI * n * k / _width));
                    sum += temp[k, y] * w;
                }

                temp[k, y] = sum / _height;
            }
        }
        
        for (var x = 0; x < _width; x++)
        {
            for (var k = 0; k < _height; k++)
            {
                var sum = Complex.Zero;
                
                for (var n = 0; n < _height; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _width), Math.Sin(2 * Math.PI * n * k / _width));
                    sum += _data[x, n] * w;
                }

                var rgb = new RGB64(sum.Magnitude / _height);
                rgb.SaveToPixel((byte*)bits.Scan0 + k * bits.Stride + x * (bits.Stride/_width));
            }
        }
        
        image.UnlockBits(bits);
        return image;
    }

    public void DiscreteFourierTransform()
    {
        if (_fourierTransformed) return;

        var temp = new Complex[_width, _height];
        
        for (var y = 0; y < _height; y++)
        {
            for (var k = 0; k < _width; k++)
            {
                var sum = Complex.Zero;
                for (var n = 0; n < _width; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _width), -Math.Sin(2 * Math.PI * n * k / _width));
                    sum += _data[n, y] * w;
                }

                temp[k, y] = sum;
            }
        }

        for (var x = 0; x < _width; x++)
        {
            for (var k = 0; k < _height; k++)
            {
                var sum = Complex.Zero;
                for (var n = 0; n < _height; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _width), -Math.Sin(2 * Math.PI * n * k / _width));
                    sum += temp[x, n] * w;
                }

                _data[x, k] = sum;
            }
        }

        _fourierTransformed = true;
    }
    
    public void PerformFFT()
    {
        var result = new List<List<Complex>>();
        var temp = new List<List<Complex>>();

        for (var row = 0; row < _height; row++)
        {
            var rowData = new Complex[_height];

            for (var x = 0; x < _width; x++)
            {
                rowData[x] = _data[x, row];
            }

            temp.Add(FFTFrequency(rowData).ToList());
        }

        for (var col = 0; col < _width; col++)
        {
            var colData = new Complex[_width];

            for (var y = 0; y < _height; y++)
            {
                colData[y] = temp[y][col];
            }

            result.Add(FFTFrequency(colData).ToList());
        }

        for (var i = 0; i < result.Count; i++)
        {
            List<Complex> list = result[i];
            for (var j = 0; j < list.Count; j++)
            {
                Complex complex = list[j];
                _data[i, j] = complex;
            }
        }
    }

    private Complex[] FFTFrequency(Complex[] signal)
    {
        if (signal.Length == 1)
        {
            return signal;
        }

        var result = new Complex[signal.Length];

        var even = new Complex[signal.Length / 2];
        var odd = new Complex[signal.Length / 2];


        for (var i = 0; i < signal.Length / 2; i++)
        {
            var number = new Complex(Math.Cos(2 * Math.PI * i / signal.Length), -Math.Sin(2 * Math.PI * i / signal.Length));
            even[i] = signal[i] + signal[i + signal.Length / 2];
            odd[i] = (signal[i] - signal[i + signal.Length / 2]) * number;
        }

        even = FFTFrequency(even);
        odd = FFTFrequency(odd);

        for (var i = 0; i < signal.Length / 2; i++)
        {
            result[2*i] = even[i];
            result[2*i + 1] = odd[i];
        }

        return result;
    }
    
    public unsafe Bitmap InvertFFT()
    {
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
        BitmapData bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)), 
            ImageLockMode.ReadWrite, image.PixelFormat);
        var temp = new List<List<Complex>>();

        for (var x = 0; x < _height; x++)
        {
            temp.Add(new List<Complex>());
        }

        for (var y = 0; y < _height; y++)
        {
            var rowData = new Complex[_height];

            for (var x = 0; x < image.Width; x++)
            {
                rowData[x] = _data[x, y];
            }

            rowData = IFFTFrequency(rowData);

            for (var x = 0; x < _width; x++)
            {
                temp[y].Add(rowData[x] / image.Width);
            }
        }

        for (var x = 0; x < _width; x++)
        {
            var colData = new Complex[_height];

            for (var y = 0; y < _width; y++)
            {
                colData[y] = temp[y][x];
            }

            colData = IFFTFrequency(colData);

            for (var y = 0; y < _height; y++)
            {
                var rgb = new RGB64(Math.Abs(colData[y].Magnitude / image.Width));
                rgb.SaveToPixel((byte*)bits.Scan0 + y * bits.Stride + x * (bits.Stride / _width));
            }
        }
        
        image.UnlockBits(bits);
        return image;
    }
    
    private Complex[] IFFTFrequency(Complex[] signal)
    {
        if (signal.Length == 1)
        {
            return signal;
        }

        var result = new Complex[signal.Length];

        var even = new Complex[signal.Length / 2];
        var odd = new Complex[signal.Length / 2];

        for (var i = 0; i < signal.Length / 2; i++)
        {
            var number = new Complex(Math.Cos(2 * Math.PI * i / signal.Length), Math.Sin(2 * Math.PI * i / signal.Length));
            even[i] = signal[i] + signal[i + signal.Length / 2];
            odd[i] = (signal[i] - signal[i + signal.Length / 2]) * number;
        }

        even = IFFTFrequency(even);
        odd = IFFTFrequency(odd);

        for (var i = 0; i < signal.Length / 2; i++)
        {
            result[i * 2] = even[i];
            result[i * 2 + 1] = odd[i];
        }

        return result;
    }

    public void SwapQuarters()
    {
        var result = _data;
        
        for (var x = 0; x < _width / 2; x++)
        {
            for (var y = 0; y < _height / 2; y++)
            {
                var temp = new Complex(result[x, y].Real, result[x, y].Imaginary);
                result[x, y] = result[_width / 2 + x, _height / 2 + y];
                result[_width / 2 + x, _height / 2 + y] = temp;

                temp = new Complex(result[_width / 2 + x, y].Real, result[_width / 2 + x, y].Imaginary);
                result[_width / 2 + x, y] = result[x, _width / 2 + y];
                result[x, _height / 2 + y] = temp;
            }
        }

        _data = result;
    }

    public unsafe Bitmap VisualizeFourierSpectrum()
    {
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
        var bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)), 
            ImageLockMode.ReadWrite, image.PixelFormat);
            
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                var rgb = new RGB64(Math.Log(Math.Abs(_data[y, x].Magnitude)) * 10);
                rgb.SaveToPixel((byte*)bits.Scan0 + y * bits.Stride + x * (bits.Stride / _width));
            }
        }
        image.UnlockBits(bits);
        return image;
    }
}