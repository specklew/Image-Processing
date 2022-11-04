namespace image_processing_core;

public partial class RGBComparer : IComparer<RGB64>
{
    public int Compare(RGB64 x, RGB64 y)
    {
        return x.Length() > y.Length() ? 1 : 0;
    }
}

public partial class RGBComparer : IComparer<RGB>
{
    public int Compare(RGB x, RGB y)
    {
        return x.Length() > y.Length() ? 1 : 0;
    }
}