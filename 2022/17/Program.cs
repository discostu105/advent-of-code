using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

var input = File.ReadAllText("test.txt");

var plusShape = new Shape('+', new bool[,] {
    { false, true, false },
    { true, true, true },
    { false, true, false },
});

var dashShape = new Shape('@', new bool[,] {
    { true, true, true, true }
});

var lShape = new Shape('L', new bool[,] {
    { true, true, true },
    { false, false, true },
    { false, false, true },
});

var iShape = new Shape('I', new bool[,] {
    { true },
    { true  },
    { true  },
    { true  },
});

var squareShape = new Shape('O', new bool[,] {
    { true, true },
    { true, true },
});

Console.WriteLine(Simulate(input, 2022));
Console.WriteLine(Simulate(input, 1000000000000));

long Simulate(string moves, long numberOfShapes) {
    var maxMoves = moves.Length;
    var cave = new Cave(7);
    var sw = new Stopwatch();
    sw.Start();
    int reportInterval = 1000000;

    var movePos = 0;
    var shapes = EnumerateShapes().GetEnumerator();
    int cacheHits = 0;
    for (long i = 0; i < numberOfShapes; i++) {
        shapes.MoveNext();
        var shape = shapes.Current;

        cave.SpawnShape(shape);
        //cave.Print(shape);
        cave.DropShape(moves, maxMoves, ref movePos, ref cacheHits, shape);
        //cave.Print();
        if (i % reportInterval == 0 || i == numberOfShapes - 1) {
            var elapsed = sw.Elapsed;
            sw.Restart();
            var remaining = TimeSpan.FromSeconds(elapsed.TotalSeconds * ((double)numberOfShapes / reportInterval));
            var cacheHitRate = Math.Round(cacheHits / (double)i, 4);
            Console.WriteLine($"At block {i}, height is {cave.Height}, actualheight: {cave.ActualHeight}. Took {elapsed}. Remaining time: {remaining}. cacheHitRate: {cacheHitRate}, cacheSize: {cave.cache.Count}");

        }
    }
    cave.Print();
    return cave.ActualHeight;
}


IEnumerable<Shape> EnumerateShapes() {
    while (true) {
        yield return dashShape;
        yield return plusShape;
        yield return lShape;
        yield return iShape;
        yield return squareShape;
    }
}

class CacheKey {

    public CacheKey(char shapeSymbol, int movePos, long[] caveTopY) {
        ShapeSymbol = shapeSymbol;
        MovePos = movePos;
        CaveTopY = caveTopY;
    }

    public char ShapeSymbol { get; }
    public int MovePos { get; }
    public long[] CaveTopY { get; }

    public override int GetHashCode() {
        var hash = new HashCode();
        hash.Add(ShapeSymbol);
        hash.Add(MovePos);
        foreach (var item in CaveTopY) hash.Add(item);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj) {
        var other = obj as CacheKey;
        if (other == null) return false;
        return this.ShapeSymbol == other.ShapeSymbol &&
            this.MovePos == other.MovePos &&
            this.CaveTopY.SequenceEqual(other.CaveTopY);
    }
}
record CacheEntry(long[] newCaveTopY, long normalizeYValue, int usedMoves);

class Shape {
    // coordinate in cave of bottom-left
    public long CaveX { get; set; }
    public long CaveY { get; set; }
    public char Symbol { get; init; }

    public bool[,] Rows;

    public int Width { get; set; }
    public int Height { get; set; }

    public Shape(char symbol, bool[,] rows) {
        Symbol = symbol;
        Rows = rows;
        Width = rows.GetLength(1);
        Height = rows.GetLength(0);
    }


    public bool HitLocal(long x, long y) {
        if (x < 0 || x >= Width) return false;
        if (y < 0 || y >= Height) return false;
        return Rows[y, x];
    }

    public bool HitGlobal(long x, long y) {
        return HitLocal(x - CaveX, y - CaveY);
    }
}


class Cave {

    public Dictionary<CacheKey, CacheEntry> cache = new Dictionary<CacheKey, CacheEntry>();
    private int width;
    private long maxY = 0;
    private long[] topY;
    public long Height { get => maxY; }

    private long normalizedY = 0;

    public long ActualHeight { get => maxY + normalizedY; }

    public Cave(int width) {
        this.width = width;
        this.topY = new long[width];
    }

    public void DropShape(string moves, int maxMoves, ref int movePos, ref int cacheHits, Shape shape) {
        var topYCopy = new long[width];
        Array.Copy(topY, topYCopy, width);
        var cacheKey = new CacheKey(shape.Symbol, movePos, topYCopy);
        
        if (cache.ContainsKey(cacheKey)) {
            var cacheEntry = cache[cacheKey];
            topY = cacheEntry.newCaveTopY.ToArray();
            normalizedY += cacheEntry.normalizeYValue;
            maxY = topY.Max();
            movePos += cacheEntry.usedMoves;
            movePos %= maxMoves;
            cacheHits++;
        } else {
            var normalizedYBefore = normalizedY;
            var usedMoves = 0;
            while (true) {
                TryPush(shape, moves[movePos++]);
                usedMoves++;
                movePos %= maxMoves;
                //cave.Print(shape);
                if (!TryFall(shape)) break;
                //cave.Print(shape);
            }
            //cave.Print(shape);
            FixateShape(shape);

            var cacheEntry = new CacheEntry(topY.ToArray(), normalizedY - normalizedYBefore, usedMoves);
            cache.Add(cacheKey, cacheEntry);
        }
    }

    internal void FixateShape(Shape shape) {
        for (int shapeY = 0; shapeY < shape.Height; shapeY++) {
            var caveY = shape.CaveY + shapeY;
            for (int shapeX = 0; shapeX < shape.Width; shapeX++) {
                if (shape.Rows[shapeY, shapeX]) {
                    var caveX = shape.CaveX + shapeX;
                    topY[caveX] = Math.Max(topY[caveX], caveY + 1);
                }
            }
        }
        var normalizeValue = GetNormalizeValue();
        NormalizeBy(normalizeValue);
        maxY = topY.Max();
    }

    private long GetNormalizeValue() {
        return topY.Min();
    }

    private void NormalizeBy(long normalizeValue) {
        for (int x = 0; x < width; x++) {
            topY[x] -= normalizeValue;
        }
        normalizedY += normalizeValue;
    }

    internal void SpawnShape(Shape shape) {
        (shape.CaveX, shape.CaveY) = FindSpawnPosition(shape);
    }

    internal bool TryFall(Shape shape) {
        var fallCoordinates = (shape.CaveX, CaveY: shape.CaveY - 1);
        bool canFall = (fallCoordinates.CaveY - 1 > maxY) || TestPosition(shape, fallCoordinates);
        if (!canFall) return false;
        ApplyShapePosition(shape, fallCoordinates);
        return true;
    }

    private bool TestPosition(Shape shape, (long X, long Y) fallCoordinates) {
        if (fallCoordinates.X < 0 || fallCoordinates.X + shape.Width > width) return false;
        if (fallCoordinates.Y < 0) return false;
        var movedShape = new Shape(shape.Symbol, shape.Rows) {
            CaveX = fallCoordinates.X,
            CaveY = fallCoordinates.Y
        };
        for (int y = 0; y < shape.Height; y++) {
            for (int x = 0; x < shape.Width; x++) {
                if (shape.HitLocal(x, y) && !this.IsFree(movedShape.CaveX + x, movedShape.CaveY + y))
                    return false;
            }
        }
        return true;
    }

    internal bool TryPush(Shape shape, char direction) {
        var pushCoordinates = direction == '>'
            ? (shape.CaveX + 1, shape.CaveY)
            : (shape.CaveX - 1, shape.CaveY);
        bool canPush = TestPosition(shape, pushCoordinates);
        if (!canPush) return false;
        ApplyShapePosition(shape, pushCoordinates);
        return true;
    }

    private void ApplyShapePosition(Shape shape, (long X, long Y) fallCoordinates) {
        shape.CaveX = fallCoordinates.X;
        shape.CaveY = fallCoordinates.Y;
    }

    private (long x, long y) FindSpawnPosition(Shape shape) {
        return (2, maxY + 3);
    }

    internal void Print(Shape shape = null) {
        Console.WriteLine("TopY: " + string.Join(", ", topY));

        var height = shape == null ? this.Height : Math.Max(this.Height, shape.CaveY + shape.Height);
        for (long y = height - 1; y >= 0; y--) {
            Console.Write('|');
            for (long x = 0; x < width; x++) {
                if (shape != null && shape.HitGlobal(x, y)) {
                    Console.Write(shape.Symbol);
                } else if (!IsFree(x, y)) {
                    Console.Write('#');
                } else {
                    Console.Write('.');
                }
            }
            Console.Write('|');
            Console.WriteLine();
        }
        Console.Write('+');
        for (int x = 0; x < width; x++) {
            Console.Write('-');
        }
        Console.Write('+');
        Console.WriteLine();
    }

    private bool IsFree(long x, long y) {
        return topY[x] <= y;
    }
}
