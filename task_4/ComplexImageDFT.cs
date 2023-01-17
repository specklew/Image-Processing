using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing_core;

namespace task_4;

public partial class ComplexImage
{
    public unsafe Bitmap InvertDFT()
    {
        var image = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);
        var bits = image.LockBits(
            new Rectangle(Point.Empty, new Size(_width, _height)),
            ImageLockMode.ReadWrite, image.PixelFormat);

        Complex[,] temp = new Complex[_width, _height];

        for (var y = 0; y < _height; y++)
        {
            for (var k = 0; k < _width; k++)
            {
                var sum = Complex.Zero;
                for (var n = 0; n < _width; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _width), Math.Sin(2 * Math.PI * n * k / _width));
                    sum += _data[n, y] * w;
                }

                temp[k, y] = sum / Math.Sqrt(_height);
            }
        }

        for (var x = 0; x < _width; x++)
        {
            for (var k = 0; k < _height; k++)
            {
                var sum = Complex.Zero;

                for (var n = 0; n < _height; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _height),
                        Math.Sin(2 * Math.PI * n * k / _height));
                    sum += temp[x, n] * w;
                }

                var rgb = new RGB64(sum.Magnitude / Math.Sqrt(_width));
                rgb.SaveToPixel((byte*) bits.Scan0 + k * bits.Stride + x * (bits.Stride / _width));
            }
        }

        image.UnlockBits(bits);
        return image;
    }

    public void PerformDFT()
    {
        if (_fourierTransformed) return;

        Complex[,] temp = new Complex[_width, _height];

        for (var x = 0; x < _width; x++)
        {
            for (var k = 0; k < _height; k++)
            {
                var sum = Complex.Zero;
                for (var n = 0; n < _height; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _height),
                        -Math.Sin(2 * Math.PI * n * k / _height));
                    sum += _data[x, n] * w;
                }

                temp[x, k] = sum;
            }
        }

        for (var y = 0; y < _height; y++)
        {
            for (var k = 0; k < _width; k++)
            {
                var sum = Complex.Zero;
                for (var n = 0; n < _width; n++)
                {
                    var w = new Complex(Math.Cos(2 * Math.PI * n * k / _width),
                        -Math.Sin(2 * Math.PI * n * k / _width));
                    sum += temp[n, y] * w;
                }

                _data[k, y] = sum;
            }
        }

        _fourierTransformed = true;
    }
}