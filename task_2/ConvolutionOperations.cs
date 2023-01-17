using System.Drawing;
using System.Drawing.Imaging;
using image_processing;
using image_processing_core;

namespace task_2;

public static class ConvolutionOperations
{

    private static readonly int[,] Laplace1 =
    {
        { 0,-1, 0},
        {-1, 4,-1},
        { 0,-1, 0}
    };
    
    private static readonly int[,] Laplace2 =
    {
        {-1,-1,-1},
        {-1, 8,-1},
        {-1,-1,-1}
    };
    
    private static readonly int[,] Laplace3 =
    {
        { 1,-2, 1},
        {-2, 4,-2},
        { 1,-2, 1}
    };

    private static readonly int[,] RegionWhatever =
    {
        {-1, 1, 1},
        {-1,-2, 1},
        {-1, 1, 1}
    };
    
    private static readonly (int, int)[] Steps =
    {
        (1, 0),
        (0, 1),
        (-1,0),
        (0,-1)
    };

    public static Bitmap ExtractDetails(Bitmap bitmap, int kernelSelection)
    {
        var newBitmap = new Bitmap(bitmap);
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        int[,] selectedKernel = kernelSelection switch
        {
            1 => Laplace2,
            2 => Laplace3,
            _ => Laplace1
        };

        newData = PerformConvolution(data, newData, selectedKernel, 1f);
        newBitmap.UnlockBits(newData);

        return newBitmap;
    }

    public static BitmapData PerformConvolution(BitmapData data, BitmapData newData, int[,] kernel, float multiplier)
    {
        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                ApplyKernelToPosition(data, newData, new Point(x, y), kernel, multiplier);
            }
        }
        
        return newData;
    }

    private static unsafe void ApplyKernelToPosition(BitmapData data, BitmapData newData, Point pos, int[,] kernel, float multiplier)
    {
        var pt = (byte*)data.Scan0;
        int bpp = data.Stride / data.Width;
        
        int offsetYLeftTop = (kernel.GetLength(0) - 1) / 2;
        int offsetYRightBottom = kernel.GetLength(0) / 2;

        RGB64 result = new RGB64(0, 0, 0);
        
        for (int dy = -offsetYLeftTop; dy <= offsetYRightBottom; dy++)
        {
            int y = dy + pos.Y;

            int offsetXLeftTop = (kernel.GetLength(1) - 1) / 2;
            int offsetXRightBottom = kernel.GetLength(1) / 2;
             
            for (int dx = -offsetXLeftTop; dx <= offsetXRightBottom; dx++)
            {
                int x = dx + pos.X;
                if(x < 0 || x >= data.Width || y < 0 || y >= data.Height)
                {
                    byte* pixel = pt + pos.Y * data.Stride + pos.X * bpp;
                
                    RGB64 rgb = RGB64.ToRGB(pixel);
                    
                    rgb *= kernel[dx + offsetXLeftTop, dy + offsetYLeftTop];
                    rgb *= multiplier;

                    result += rgb;
                }
                else
                {
                    byte* pixel = pt + y * data.Stride + x * bpp;

                    RGB64 rgb = RGB64.ToRGB(pixel);

                    rgb *= kernel[dx + offsetXLeftTop, dy + offsetYLeftTop];
                    rgb *= multiplier;

                    result += rgb;
                }
            }
        }

        byte* finalPixel = (byte*)newData.Scan0 + pos.Y * newData.Stride + pos.X * (newData.Stride / newData.Width);
        result.SaveToPixel(finalPixel);
    }

    public static Bitmap OptimizedEdgeDetection(Bitmap bitmap)
    {
        var newBitmap = bitmap.Clone() as Bitmap;
        BitmapData data = ImageIO.LockPixels(bitmap);
        BitmapData newData = ImageIO.LockPixels(newBitmap);

        newData = ApplyOptimizedEdgeDetection(data, newData);    
        newBitmap.UnlockBits(newData);
        return newBitmap;
    }
    
    public static unsafe BitmapData ApplyOptimizedEdgeDetection(BitmapData data, BitmapData newData)
    {
        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = (byte*)data.Scan0 + y * data.Stride + x * bpp;
                RGB64 sum = RGB64.ToRGB(pixel);
                sum *= 4;
                
                for (var i = 0; i < 4; i++)
                {
                    int nx = x + (int)Math.Round(Math.Sin(i * 0.5 * Math.PI));
                    int ny = y + (int)Math.Round(Math.Cos(i * 0.5 * Math.PI));
                    
                    if(nx < 0 || nx >= data.Width || ny < 0 || ny >= data.Height)
                    {
                        sum -= RGB64.ToRGB(pixel);
                    }
                    else
                    {
                        sum -= RGB64.ToRGB((byte*)data.Scan0 + ny * data.Stride + nx * bpp);
                    }
                }

                byte* finalPixel = (byte*)newData.Scan0 + y * data.Stride + x * bpp;
                sum.SaveToPixel(finalPixel);
            }
        }

        return newData;
    }
    
    public static unsafe BitmapData ApplyAllocationHeavyOptimizedEdgeDetection(BitmapData data, BitmapData newData)
    {

        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = (byte*)data.Scan0 + y * data.Stride + x * bpp;
                RGB64 sum = RGB64.ToRGB(pixel);
                sum *= 4;
                
                for (var i = 0; i < 4; i++)
                {
                    int nx = x + Steps[i].Item1;
                    int ny = y + Steps[i].Item2;
                    
                    if(nx < 0 || nx >= data.Width || ny < 0 || ny >= data.Height)
                    {
                        sum -= RGB64.ToRGB(pixel);
                    }
                    else
                    {
                        sum -= RGB64.ToRGB((byte*)data.Scan0 + ny * data.Stride + nx * bpp);
                    }
                }

                byte* finalPixel = (byte*)newData.Scan0 + y * data.Stride + x * bpp;
                sum.SaveToPixel(finalPixel);
            }
        }

        return newData;
    }
    
    public static unsafe BitmapData ApplyOprimizedDigonalConvolution(BitmapData data, BitmapData newData)
    {

        int bpp = data.Stride / data.Width;

        for (var y = 0; y < data.Height; y++)
        {
            for (var x = 0; x < data.Width; x++)
            {
                byte* pixel = (byte*)data.Scan0 + y * data.Stride + x * bpp;
                RGB64 sum = RGB64.ToRGB(pixel);
                sum *= 4;
                
                for (var i = 0; i < 4; i++)
                {
                    int nx = x + Steps[i].Item1;
                    int ny = y + Steps[i].Item2;
                    
                    if(nx < 0 || nx >= data.Width || ny < 0 || ny >= data.Height)
                    {
                        sum -= RGB64.ToRGB(pixel);
                    }
                    else
                    {
                        sum -= RGB64.ToRGB((byte*)data.Scan0 + ny * data.Stride + nx * bpp);
                    }
                }

                byte* finalPixel = (byte*)newData.Scan0 + y * data.Stride + x * bpp;
                sum.SaveToPixel(finalPixel);
            }
        }

        return newData;
    }

    public static Bitmap ApplyOptimizedBitmapConvolution(Bitmap bitmap)
    {
        Bitmap result = (bitmap.Clone() as Bitmap)!;

        Color[] cache1 = new Color[3];
        Color[] cache2 = new Color[3];

        for (int x = 0; x < bitmap.Width; x++)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                result = ApplyKernelToPosition(bitmap, result, x, y, ref cache1, ref cache2);
            }
        }
        return result;
    }

    private static Bitmap ApplyKernelToPosition(Bitmap initial, Bitmap result, int x, int y, ref Color[] cache1, ref Color[] cache2)
    {
        int r = 0, g = 0, b = 0;

        for (int dx = 0; dx <= 2; dx++)
        {
            for (int dy = 0; dy <= 2; dy++)
            {
                int kx = x + dx - 1;
                int ky = y + dy - 1;

                if (kx <= 0 || kx >= initial.Width || ky <= 0 || ky >= initial.Height) continue;
                
                /*Color pixel = initial.GetPixel(kx, ky);
                r += pixel.R * RegionWhatever[dx, dy];
                g += pixel.G * RegionWhatever[dx, dy];
                b += pixel.B * RegionWhatever[dx, dy];*/
                
                if (dx == 0)
                {
                    if (cache2[dy] == Color.Black)
                    {
                        Color pixel = initial.GetPixel(kx, ky);
                        cache2[dy] = pixel;
                        r -= pixel.R;
                        g -= pixel.G;
                        b -= pixel.B;
                    }
                    else
                    {
                        Color pixel = cache2[dy];
                        r -= pixel.R;
                        g -= pixel.G;
                        b -= pixel.B;
                    }
                }
                else if (dx == 1)
                {
                    if (cache1[dy] == Color.Black)
                    {
                        Color pixel = initial.GetPixel(kx, ky);
                        cache2[dy] = pixel; 
                        r += pixel.R;
                        g += pixel.G;
                        b += pixel.B;
                    }
                    else
                    {
                        Color pixel = cache1[dy];
                        cache2[dy] = cache1[dy]; 
                        r += pixel.R;
                        g += pixel.G;
                        b += pixel.B;
                    }
                }
                else
                {
                    Color pixel = initial.GetPixel(kx, ky);
                    cache1[dy] = pixel;
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                }

                if (dx == 1 && dy == 1)
                {
                    if (cache1[dy] == Color.Black)
                    {
                        Color pixel = initial.GetPixel(kx, ky);
                        cache2[dy] = pixel; 
                        r -= pixel.R * 3;
                        g -= pixel.G * 3;
                        b -= pixel.B * 3;
                    }
                    else
                    {
                        Color pixel = cache1[dy];
                        cache2[dy] = cache1[dy];
                        r -= pixel.R * 3;
                        g -= pixel.G * 3;
                        b -= pixel.B * 3;
                    }
                }
            }
        }

        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);
        
        result.SetPixel(x, y, Color.FromArgb(r, g, b));
        
        return result;
    }
}