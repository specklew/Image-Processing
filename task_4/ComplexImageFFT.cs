using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing_core;

namespace task_4;

public partial class ComplexImage
{
    public void PerformFFT()
    {
        List<List<Complex>> result = new();
        List<List<Complex>> temp = new();

        for (var x = 0; x < _width; x++)
        {
            var xData = new Complex[_width];

            for (var y = 0; y < _height; y++)
            {
                xData[y] = _data[x, y];
            }

            temp.Add(FFTFrequency(xData).ToList());
        }
        
        for (var y = 0; y < _height; y++)
        {
            var yData = new Complex[_height];

            for (var x = 0; x < _width; x++)
            {
                yData[x] = temp[x][y] / Math.Sqrt(_width);
            }

            result.Add(FFTFrequency(yData).ToList());
        }

        for (var x = 0; x < result.Count; x++)
        {
            List<Complex> list = result[x];
            for (var y = 0; y < list.Count; y++)
            {
                Complex complex = list[y];
                _data[x, y] = complex / Math.Sqrt(_height);
            }
        }

        _fourierTransformed = true;
    }
    
    public void PerformFFTUsingSpatial()
    {
        List<List<Complex>> result = new();
        List<List<Complex>> temp = new();

        for (var x = 0; x < _width; x++)
        {
            var xData = new Complex[_width];

            for (var y = 0; y < _height; y++)
            {
                xData[y] = _data[x, y];
            }

            temp.Add(FFTSpatial(xData).ToList());
        }
        
        for (var y = 0; y < _height; y++)
        {
            var yData = new Complex[_height];

            for (var x = 0; x < _width; x++)
            {
                yData[x] = temp[x][y] / Math.Sqrt(_width);
            }

            result.Add(FFTSpatial(yData).ToList());
        }

        for (var x = 0; x < result.Count; x++)
        {
            List<Complex> list = result[x];
            for (var y = 0; y < list.Count; y++)
            {
                Complex complex = list[y];
                _data[x, y] = complex / Math.Sqrt(_height);
            }
        }

        _fourierTransformed = true;
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

    private Complex[] FFTSpatial(Complex[] signal)
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
            even[i] = signal[2 * i];
            odd[i] = signal[2 * i + 1];
        }

        even = FFTSpatial(even);
        odd = FFTSpatial(odd);
        
        for (var i = 0; i < signal.Length / 2; i++)
        {
            var number = new Complex(Math.Cos(2 * Math.PI * i / signal.Length), -Math.Sin(2 * Math.PI * i / signal.Length));
            result[i] = even[i] + number * odd[i];
            result[i + signal.Length / 2] = even[i] - number * odd[i];
        }

        return result;
    }
    
    public unsafe Bitmap InvertFFT()
    {
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
        BitmapData bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)), 
            ImageLockMode.ReadWrite, image.PixelFormat);
        List<List<Complex>> temp = new();

        for (var y = 0; y < _height; y++)
        {
            temp.Add(new List<Complex>());
        }

        for (var y = 0; y < _height; y++)
        {
            var yData = new Complex[_height];

            for (var x = 0; x < _width; x++)
            {
                yData[x] = _data[y, x];
            }

            yData = IFFTFrequency(yData);

            for (var x = 0; x < _width; x++)
            {
                temp[y].Add(yData[x] / Math.Sqrt(_height));
            }
        }

        for (var x = 0; x < _width; x++)
        {
            var xData = new Complex[_height];

            for (var y = 0; y < _width; y++)
            {
                xData[y] = temp[y][x];
            }

            xData = IFFTFrequency(xData);

            for (var y = 0; y < _height; y++)
            {
                var rgb = new RGB64(Math.Abs(xData[y].Magnitude / Math.Sqrt(_width)));
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
}