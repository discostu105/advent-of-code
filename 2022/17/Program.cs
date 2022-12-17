using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

var input = File.ReadAllText("input.txt");

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

int Simulate(string moves, long numberOfShapes) {
    var maxMoves = moves.Length;
    var cave = new Cave(7);
    var sw = new Stopwatch();
    sw.Start();
    int reportInterval = 10000000;

    var movePos = 0;
    var shapes = EnumerateShapes().GetEnumerator();
    for (long i = 0; i < numberOfShapes; i++) {
        shapes.MoveNext();
        var shape = shapes.Current;
        cave.SpawnShape(shape);
        //cave.Print(shape);
        while (true) {
            cave.TryPush(shape, moves[movePos++]);
            movePos %= maxMoves;
            //cave.Print(shape);
            if (!cave.TryFall(shape)) break;
            //cave.Print(shape);
        }
        //cave.Print(shape);
        cave.FixateShape(shape);
        //cave.Print();
        if (i % reportInterval == 0 || i == numberOfShapes - 1) {
            var elapsed = sw.Elapsed;
            sw.Restart();
            var remaining = TimeSpan.FromSeconds(elapsed.TotalSeconds * ((double)numberOfShapes / reportInterval));
            Console.WriteLine($"At block {i}, height is {cave.Height}. Took {elapsed}. Remaining time: {remaining}.");

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

class Shape {
    // coordinate in cave of bottom-left
    public Point CaveCoordinates { get; set; }
    public char Symbol { get; init; }
    //public string[] Sprite { get; init; }

    public bool[,] Rows;

    public int Width { get; set; }
    public int Height { get; set; }

    public Shape(char symbol, bool[,] rows) {
        Symbol = symbol;
        Rows = rows;
        Width = rows.GetLength(1);
        Height = rows.GetLength(0);
    }


    public bool HitLocal(int x, int y) {
        if (x < 0 || x >= Width) return false;
        if (y < 0 || y >= Height) return false;
        return Rows[y, x];
    }

    public bool HitGlobal(int x, int y) {
        return HitLocal(x - CaveCoordinates.X, y - CaveCoordinates.Y);
    }
}

class Cave {
    private int width;
    //public Dictionary<int, BitArray> CaveRows = new Dictionary<int, BitArray>();
    private int maxY = 0;
    private int[] topY;
    public int Height { get => maxY; }

    private int normalizedY = 0;

    public int ActualHeight { get => maxY + normalizedY; }

    public Cave(int width) {
        this.width = width;
        this.topY = new int[width];
    }

    internal void FixateShape(Shape shape) {
        //foreach(var row in shape.Rows) {
        //    var y = shape.CaveCoordinates.Y + row.Key;
        //    if (!CaveRows.ContainsKey(y)) {
        //        CaveRows.Add(y, new BitArray(7, false));
        //    }
        //    for (int x = 0; x < row.Value.Count; x++) {
        //        CaveRows[y][shape.CaveCoordinates.X + x] = row.Value[x];
        //    }
        //    if (CaveRows[y].Cast<bool>().All(x => x)) {
        //        //Console.WriteLine("removing rows below " + y);
        //        foreach (var rowToRemove in CaveRows.Keys.Where(row => row < y).ToList()) {
        //            CaveRows.Remove(rowToRemove);
        //        }
        //    }
        //}
        //maxY = Math.Max(maxY, shape.CaveCoordinates.Y + shape.Height);
        for (int shapeY = 0; shapeY < shape.Height; shapeY++) {
            var caveY = shape.CaveCoordinates.Y + shapeY;
            for (int shapeX = 0; shapeX < shape.Width; shapeX++) {
                if (shape.Rows[shapeY, shapeX]) {
                    var caveX = shape.CaveCoordinates.X + shapeX;
                    topY[caveX] = Math.Max(topY[caveX], caveY + 1);
                }
            }
        }
        //maxY = Math.Max(maxY, shape.CaveCoordinates.Y + shape.Height);

        Normalize();
        maxY = topY.Max();
    }

    private void Normalize() {
        var minY = topY.Min();
        for (int x = 0; x < width; x++) {
            topY[x] -= minY;
        }
        normalizedY += minY;
    }

    internal void SpawnShape(Shape shape) {
        shape.CaveCoordinates = FindSpawnPosition(shape);
    }

    internal bool TryFall(Shape shape) {
        //Console.WriteLine("Falling");

        var fallCoordinates = new Point(shape.CaveCoordinates.X, shape.CaveCoordinates.Y - 1);
        bool canFall = (fallCoordinates.Y - 1 > maxY) || TestPosition(shape, fallCoordinates);
        if (!canFall) return false;
        ApplyShapePosition(shape, fallCoordinates);
        return true;
    }

    private bool TestPosition(Shape shape, Point fallCoordinates) {
        //if (fallCoordinates.Y < heightOffset) return false;
        if (fallCoordinates.X < 0 || fallCoordinates.X + shape.Width > width) return false;
        if (fallCoordinates.Y < 0) return false;
        //if (HitBlocked(fallCoordinates)) return false;
        var movedShape = new Shape(shape.Symbol, shape.Rows) {
            CaveCoordinates = fallCoordinates
        };
        for (int y = 0; y < shape.Height; y++) {
            for (int x = 0; x < shape.Width; x++) {
                if (shape.HitLocal(x, y) && !this.IsFree(movedShape.CaveCoordinates.X + x, movedShape.CaveCoordinates.Y + y))
                    return false;
            }
        }
        return true;
    }

    internal bool TryPush(Shape shape, char direction) {
        //Console.WriteLine("Pushing towards " + direction);
        var pushCoordinates = direction == '>'
            ? new Point(shape.CaveCoordinates.X + 1, shape.CaveCoordinates.Y)
            : new Point(shape.CaveCoordinates.X - 1, shape.CaveCoordinates.Y);
        bool canPush = TestPosition(shape, pushCoordinates);
        if (!canPush) return false;
        ApplyShapePosition(shape, pushCoordinates);
        return true;
    }

    private void ApplyShapePosition(Shape shape, Point fallCoordinates) {
        shape.CaveCoordinates = fallCoordinates;
    }

    private Point FindSpawnPosition(Shape shape) {
        //if (CaveRows.Count == 0) return new Point(2, 3);
        return new Point(2, maxY + 3);
    }

    internal void Print(Shape shape = null) {
        Console.WriteLine("TopY: " + string.Join(", ", topY));

        var height = shape == null ? this.Height : Math.Max(this.Height, shape.CaveCoordinates.Y + shape.Height);
        for (int y = height - 1; y >= 0; y--) {
            Console.Write('|');
            for (int x = 0; x < width; x++) {
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

    private bool IsFree(int x, int y) {
        return topY[x] <= y;
    }
}