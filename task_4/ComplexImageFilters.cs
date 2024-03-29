﻿using System.Drawing;
using System.Numerics;

namespace task_4;

public partial class ComplexImage
{
    public void LowpassFilter(double threshold)
    {
        if (!_fourierTransformed) throw new InvalidOperationException();

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                double distance = Math.Sqrt(Math.Pow(x - _width / 2.0, 2) + Math.Pow(y - _height / 2.0, 2));

                if (distance < threshold)
                {
                    _data[x, y] = new Complex(0, 0);
                }
            }
        }
    }
    
    
    public void HighpassFilter(double threshold)
    {
        if (!_fourierTransformed) throw new InvalidOperationException();

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                double distance = Math.Sqrt(Math.Pow(x - _width / 2.0, 2) + Math.Pow(y - _height / 2.0, 2));

                // if is below the threshold, set to 0
                if (distance > threshold)
                {
                    _data[x, y] = new Complex(0, 0);
                }
            }
        }
    }
    
    
    public void BandCutFilter(double lowThreshold, double highThreshold)
    {
        if (!_fourierTransformed) throw new InvalidOperationException();

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                double distance = Math.Sqrt(Math.Pow(x - _width / 2.0, 2) + Math.Pow(y - _height / 2.0, 2));

                if (distance <= highThreshold && distance >= lowThreshold) 
                {
                    _data[x, y] = new Complex(0, 0);
                }
            }
        }
    }
    
    public void BandPassFilter(double lowThreshold, double highThreshold)
    {
        if (!_fourierTransformed) throw new InvalidOperationException();

        HighpassFilter(highThreshold);
        LowpassFilter(lowThreshold);
    }
    
    public void HighpassFilterWithEdgeDetection(Bitmap mask)
    {
        if (!_fourierTransformed) throw new InvalidOperationException();
        
        float scaleX = _width / (float)mask.Width;
        float scaleY = _height / (float)mask.Height;

        //TODO: Investigate value swap
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                if (mask.GetPixel((int)(x / scaleX), (int)(y / scaleY)).R == 0)
                {
                    _data[_height - 1 - y, x] = new Complex(0, 0);
                }
            }
        }
    }
    
    public void PhaseModifyingFilter(int k, int l)
    {
        if (!_fourierTransformed) throw new InvalidOperationException();

        Complex[,] mask = new Complex[_width, _height];

        for (var x = 0; x < _height; x++)
        {   
            for (var y = 0; y < _width; y++)
            {
                mask[x,y] = Complex.Exp(Complex.ImaginaryOne * (-1 * (x * k * 2 * Math.PI) / _height + -1 * (y * l * 2 * Math.PI) / _width + (k + l) * Math.PI));
            }
        }

        for (var x = 0; x < _height; x++)
        {
            for (var y = 0; y < _width; y++)
            {
                _data[x, y] *= mask[x, y];
            }
        }
    }
}