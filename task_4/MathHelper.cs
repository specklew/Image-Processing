using System.Numerics;

namespace Task4;

public static class MathHelper
{
    public static Complex DFT(Complex[] signal, int n)
    {
        var sum = new Complex(0.0, 0.0);
        
        for (var k = 0; k < signal.Length; k++)
        {
            var w = new Complex(Math.Cos(2 * Math.PI * k * n / signal.Length), -Math.Sin(2 * Math.PI * k * n / signal.Length));
            sum += signal[k] * w;
        }
        return sum;
    }
}