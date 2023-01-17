using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using image_processing_core;

namespace task_4;

public partial class ComplexImage
{
    public void SwapQuarters()
    {
        Complex[,] result = _data;
        
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