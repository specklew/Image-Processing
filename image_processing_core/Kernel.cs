using System.Collections;
using image_processing;

namespace image_processing_core;

//TODO: Finish implementation.
public class Kernel : IReadOnlyCollection<int>
{
    private readonly IList<IList<int>> _values;

    public Kernel(IList<IList<int>> values)
    {
        if (values.Any(value => value.Count != values.Count))
        {
            throw new NotSupportedException();
        }

        _values = values;
    }

    private int this[int x, int y]
    {
        get => _values[x][y];
        set => _values[x][y] = value;
    }

    public IEnumerator<int> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public int Count => _values.Count;

    public struct Enumerator : IEnumerator<int>
    {
        private Kernel _kernel;
        private int _indexX;
        private int _indexY;
        public Enumerator(Kernel kernel)
        {
            _kernel = kernel;
            _indexX = 0;
            _indexY = 0;
        }
        
        public bool MoveNext()
        {
            return _indexX < _kernel.Count && _indexY < _kernel.Count;
        }

        public void Reset()
        {
            _indexX = 0;
            _indexY = 0;
        }

        public int Current => _indexX + _indexY;

        object IEnumerator.Current => _indexX + _indexY;


        public void Dispose() { }
    }
}

public static class Kernels
{
    public static readonly int[,] i =
    {
        {-1,-1,-1},
        {-1, 1, 1},
        {-1,-1,-1}
    };

    public static readonly int[,] ii =
    {
        {-1,-1,-1},
        {-1, 1,-1},
        {-1, 1,-1}
    };
    
    public static readonly int[,] iii =
    {
        { 1, 1, 1},
        { 1, 1, 1},
        { 1, 1, 1}
    };
    
    public static readonly int[,] iv =
    {
        {-1, 1,-1},
        { 1, 1, 1},
        {-1, 1,-1}
    };
    
    public static readonly int[,] v =
    {
        {-1,-1,-1},
        {-1, 1, 1},
        {-1, 1,-1}
    };
    
    public static readonly int[,] vi =
    {
        {-1,-1,-1},
        {-1, 0, 1},
        {-1, 1,-1}
    };
    
    public static readonly int[,] vii =
    {
        {-1,-1,-1},
        { 1, 1, 1},
        {-1,-1,-1}
    };
    
    public static readonly int[,] viii =
    {
        {-1,-1,-1},
        { 1, 0, 1},
        {-1,-1,-1}
    };
    
    public static readonly int[,] ix =
    {
        {-1,-1,-1},
        { 1, 1,-1},
        { 1,-1,-1}
    };
    
    public static readonly int[,] x =
    {
        {-1, 1, 1},
        {-1, 1,-1},
        {-1,-1,-1}
    };
    
    //xi
    
    public static readonly int[,] xi1 =
    {
        { 1,-1,-1},
        { 1, 0,-1},
        { 1,-1,-1}
    };
    
    public static readonly int[,] xi2 =
    {
        { 1, 1, 1},
        {-1, 0,-1},
        {-1,-1,-1}
    };
    
    public static readonly int[,] xi3 =
    {
        {-1,-1, 1},
        {-1, 0, 1},
        {-1,-1, 1}
    };
    
    public static readonly int[,] xi4 =
    {
        {-1,-1,-1},
        {-1, 0,-1},
        { 1, 1, 1}
    };
    
    //xii
    
    public static readonly int[,] xii1 =
    {
        { 0, 0, 0},
        {-1, 1,-1},
        { 1, 1, 1}
    };
    
    public static readonly int[,] xii2 =
    {
        {-1, 0, 0},
        { 1, 1, 0},
        { 1, 1,-1}
    };
    
    public static readonly int[,] xii3 =
    {
        { 1,-1, 0},
        { 1, 1, 0},
        { 1,-1, 0}
    };
    
    public static readonly int[,] xii4 =
    {
        { 1, 1,-1},
        { 1, 1, 0},
        {-1, 0, 0}
    };
    
    public static readonly int[,] xii5 =
    {
        { 1, 1, 1},
        {-1, 1,-1},
        { 0, 0, 0}
    };
    
    public static readonly int[,] xii6 =
    {
        {-1, 1, 1},
        { 0, 1, 1},
        { 0, 0,-1}
    };
    
    public static readonly int[,] xii7 =
    {
        { 0,-1, 1},
        { 0, 1, 1},
        { 0,-1, 1}
    };
    
    public static readonly int[,] xii8 =
    {
        { 0, 0,-1},
        { 0, 1, 1},
        {-1, 1, 1}
    };

    public static readonly Dictionary<string, int[,]> All = new()
    {
        {"i", i},
        {"ii", ii},
        {"iii", iii},
        {"iv", iv},
        {"v", v},
        {"vi", vi},
        {"vii", vii},
        {"viii", viii},
        {"ix", ix},
        {"x", x},
        {"xi1", xi1},
        {"xi2", xi2},
        {"xi3", xi3},
        {"xi4", xi4},
        {"xii1", xii1},
        {"xii2", xii2},
        {"xii3", xii3},
        {"xii4", xii4},
        {"xii5", xii5},
        {"xii6", xii6},
        {"xii7", xii7},
        {"xii8", xii8}
    };
    
    public static int[,] GetKernel(string name)
    {
        return All[name];
    }

    public static bool Contains(string? name)
    {
        return name != null && All.ContainsKey(name);
    }
}