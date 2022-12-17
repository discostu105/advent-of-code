using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

var input = File.ReadAllText("input.txt");

//Console.WriteLine(Simulate(input, 2022));
Console.WriteLine(Simulate(input, 1000000000000));

int Simulate(string moves, long numberOfShapes) {
    var maxMoves = moves.Length;
    var cave = new Cave(7);
    var sw = new Stopwatch();
    sw.Start();
    int reportInterval = 100000;

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
        //cave.Print(shape);
        if (i % reportInterval == 0 || i == numberOfShapes - 1) {
            var elapsed = sw.Elapsed;
            sw.Restart();
            var remaining = TimeSpan.FromSeconds(elapsed.TotalSeconds * ((double)numberOfShapes / reportInterval));
            Console.WriteLine($"At block {i}, height is {cave.Height}. Took {elapsed}. Remaining time: {remaining}. rows: {cave.CaveRows.Count}");

        }
    }
    //cave.Print();
    return cave.Height;
}

IEnumerable<Shape> EnumerateShapes() {
    while (true) {
        yield return Shape.NewDash();
        yield return Shape.NewPlus();
        yield return Shape.NewL();
        yield return Shape.NewI();
        yield return Shape.NewSquare();
    }
}

class Shape {
    // coordinate in cave of bottom-left
    public Point CaveCoordinates { get; set; }
    public char Symbol { get; init; }
    //public string[] Sprite { get; init; }

    public Dictionary<int, BitArray> Rows = new Dictionary<int, BitArray>();
    public int Width { get; set; }
    public int Height { get; set; }

    public Shape(char symbol, Dictionary<int, BitArray> rows) {
        Symbol = symbol;
        Rows = rows;
        Width = rows[0].Length;
        Height = rows.Count;
    }

    internal static Shape NewPlus() => new Shape('+', new Dictionary<int, BitArray> {
        [0] = new BitArray(new bool[] { false, true, false }),
        [1] = new BitArray(new bool[] { true, true, true }),
        [2] = new BitArray(new bool[] { false, true, false }),
    });

    internal static Shape NewDash() => new Shape('@', new Dictionary<int, BitArray> {
        [0] = new BitArray(new bool[] { true, true, true, true }),
    });

    internal static Shape NewL() => new Shape('L', new Dictionary<int, BitArray> {
        [0] = new BitArray(new bool[] { true, true, true }),
        [1] = new BitArray(new bool[] { false, false, true }),
        [2] = new BitArray(new bool[] { false, false, true }),
    });

    internal static Shape NewI() => new Shape('I', new Dictionary<int, BitArray> {
        [0] = new BitArray(new bool[] { true }),
        [1] = new BitArray(new bool[] { true }),
        [2] = new BitArray(new bool[] { true }),
        [3] = new BitArray(new bool[] { true }),
    });

    internal static Shape NewSquare() => new Shape('O', new Dictionary<int, BitArray> {
        [0] = new BitArray(new bool[] { true, true }),
        [1] = new BitArray(new bool[] { true, true }),
    });

    public bool HitLocal(Point coord) {
        if (coord.X < 0 || coord.X >= Width) return false;
        if (coord.Y < 0 || coord.Y >= Height) return false;
        return Rows[coord.Y][coord.X];
    }

    public bool HitGlobal(Point coord) {
        return HitLocal(new Point(coord.X - CaveCoordinates.X, coord.Y - CaveCoordinates.Y));
    }
}

class Cave {
    private int width;
    public Dictionary<int, BitArray> CaveRows = new Dictionary<int, BitArray>();
    private int maxY = 0;

    public int Height { get => maxY; }

    public Cave(int width) {
        this.width = width;
    }

    internal void FixateShape(Shape shape) {
        foreach(var row in shape.Rows) {
            var y = shape.CaveCoordinates.Y + row.Key;
            if (!CaveRows.ContainsKey(y)) {
                CaveRows.Add(y, new BitArray(7, false));
            }
            for (int x = 0; x < row.Value.Count; x++) {
                CaveRows[y][shape.CaveCoordinates.X + x] = row.Value[x];
            }
            if (CaveRows[y].Cast<bool>().All(x => x)) {
                //Console.WriteLine("removing rows below " + y);
                foreach (var rowToRemove in CaveRows.Keys.Where(row => row < y).ToList()) {
                    CaveRows.Remove(rowToRemove);
                }
            }
        }
        maxY = Math.Max(maxY, shape.CaveCoordinates.Y + shape.Height);
    }

    internal void SpawnShape(Shape shape) {
        shape.CaveCoordinates = FindSpawnPosition(shape);
    }

    internal bool TryFall(Shape shape) {
        //Console.WriteLine("Falling");
        var fallCoordinates = new Point(shape.CaveCoordinates.X, shape.CaveCoordinates.Y - 1);
        bool canFall = TestPosition(shape, fallCoordinates);
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
                if (shape.HitLocal(new Point(x, y)) && !this.IsFree(new Point(movedShape.CaveCoordinates.X + x, movedShape.CaveCoordinates.Y + y)))
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
        if (CaveRows.Count == 0) return new Point(2, 3);
        return new Point(2, maxY + 3);
    }

    internal void Print(Shape shape = null) {
        var height = shape == null ? this.Height : Math.Max(this.Height, shape.CaveCoordinates.Y + shape.Height);
        for (int y = height - 1; y >= 0; y--) {
            Console.Write('|');
            for (int x = 0; x < width; x++) {
                if (shape != null && shape.HitGlobal(new Point(x, y))) {
                    Console.Write(shape.Symbol);
                } else if (!IsFree(new Point(x, y))) {
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

    private bool IsFree(Point point) {
        if (CaveRows.TryGetValue(point.Y, out var caveRow)) {
            return !caveRow[point.X];
        }
        return true;
    }
}