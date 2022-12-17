using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

var input = File.ReadAllText("input.txt");

Console.WriteLine(Simulate(input, 2022));
Console.WriteLine(Simulate(input, 1000000000000));

int Simulate(string moves, long numberOfShapes) {
    var maxMoves = moves.Length;
    var cave = new Cave(7);
    var sw = new Stopwatch();
    sw.Start();
    int reportInterval = 10000;

    var movePos = 0;
    var shapes = EnumerateShapes().GetEnumerator();
    for (long i = 0; i < numberOfShapes; i++) {
        shapes.MoveNext();
        var shape = shapes.Current;
        cave.SpawnShape(shape);
        //cave.Print();
        while (true) {
            cave.TryPush(shape, moves[movePos++]);
            movePos %= maxMoves;
            //cave.Print();
            if (!cave.TryFall(shape)) break;
            //cave.Print();
        }
        //cave.Print();
        if (i % 100 == 0) cave.Optimize();
        //cave.Print();
        if (i % reportInterval == 0) {
            var elapsed = sw.Elapsed;
            sw.Restart();
            var remaining = TimeSpan.FromSeconds(elapsed.TotalSeconds * ((double)numberOfShapes / reportInterval));
            Console.WriteLine($"At block {i}, height is {cave.Height}. Took {elapsed}. Remaining time: {remaining}.");

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

    private Dictionary<int, BitArray> Rows = new Dictionary<int, BitArray>();
    public int Width { get => Rows[0].Length; }
    public int Height { get => Rows.Coun; }

    public Shape(char symbol, string[] sprite) {
        Symbol = symbol;
        Sprite = sprite;
    }

    internal static Shape NewPlus() => new Shape('+', new[] {
        ".#.",
        "###",
        ".#.",
    });

    internal static Shape NewDash() => new Shape('@', new[] {
        "####",
    });

    internal static Shape NewL() => new Shape('L', new[] {
        "###",
        "..#",
        "..#",
    });

    internal static Shape NewI() => new Shape('I', new[] {
        "#",
        "#",
        "#",
        "#",
    });

    internal static Shape NewSquare() => new Shape('O', new[] {
        "##",
        "##",
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
    private List<Shape> shapes = new List<Shape>();
    private Dictionary<int, BitArray> rows = new Dictionary<int, BitArray>();

    public int Height { get => shapes.Max(s => s.CaveCoordinates.Y + s.Height) + heightOffset; }

    public Cave(int width) {
        this.width = width;
    }

    // remove all shapes. instead initialize a matrix of already blocked coordinates
    // if a line is fully blocked, assume everything below is "unreachable", so does not need to be stored ("heightOffset")
    internal void Optimize() {
        //var sprite = new List<char[]>();

        //for (int y = Height; y >= 0; y--) {
        //    var line = new char[width];
        //    for (int x = 0; x < width; x++) {
        //        if (!IsFree(null, new Point(x, y))) {
        //            line[x] = '#';
        //        } else {
        //            line[x] = '.';
        //        }
        //    }
        //    sprite.Add(line);
        //    if (line.All(x => x == '#')) {
        //        //Console.WriteLine(" ---- CUTOFF ---- ");
        //        break;
        //    }
        //}
        //sprite.Reverse();
        var blocked = new HashSet<Point>();
        //var mainshape = shapes.SingleOrDefault(x => x.Symbol == '#');
        foreach(var shape in shapes) {
            //if (shape == mainshape) continue;
            for (int x = 0; x < shape.Width; x++) {
                for (int y = 0; y < shape.Height; y++) {
                    if (shape.HitLocal(new Point(x, y))) blocked.Add(new Point(x + shape.CaveCoordinates.X, y + shape.CaveCoordinates.Y));
                }
            }
        }

        var maxX = blocked.Max(coord => coord.X);
        var minX = blocked.Min(coord => coord.X);
        var maxY = blocked.Max(coord => coord.Y);
        var minY = blocked.Min(coord => coord.Y);

        if (maxY - minY < 0) return;

        var sprite = new string[maxY - minY + 1];
        for (int y = minY; y < maxY - minY + 1; y++) {
            var sb = new StringBuilder(maxX + 1);
            for (int x = 0; x < width; x++) {
                sb.Append(blocked.Contains(new Point(x, y)) ? '#' : '.');
            }
            sprite[y] = sb.ToString();
        }

        shapes = new List<Shape>();
        shapes.Add(new Shape('#', sprite.Select(x => new string(x)).ToArray()));
        //if (mainshape != null) shapes.Add(mainshape);

        //var blocked = new List<bool[]>();
        //for (int y = Height; y >= 0; y--) {
        //    var row = new bool[width];
        //    for (int x = 0; x < width; x++) {
        //        if (!IsFree(null, new Point(x, y))) row[x] = true;
        //    }
        //    blocked.Add(row);
        //    if (row.All(x=>x)) {
        //        heightOffset = y;
        //        break;
        //    }
        //}
        //this.blocked = blocked;
        //this.shapes = new List<Shape>(); // clear shapes
    }

    internal void SpawnShape(Shape shape) {
        shape.CaveCoordinates = FindSpawnPosition(shape);
        shapes.Add(shape);
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
        var movedShape = new Shape(shape.Symbol, shape.Sprite) {
            CaveCoordinates = fallCoordinates
        };
        for (int y = 0; y < shape.Height; y++) {
            for (int x = 0; x < shape.Width; x++) {
                if (shape.HitLocal(new Point(x, y)) && !this.IsFree(shape, new Point(movedShape.CaveCoordinates.X + x, movedShape.CaveCoordinates.Y + y)))
                    return false;
            }
        }
        return true;
    }

    //private bool HitBlocked(Point fallCoordinates) {
    //    if (fallCoordinates.Y - heightOffset >= blocked.Count) return false;
    //    return blocked[fallCoordinates.Y - heightOffset][fallCoordinates.X];
    //}

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
        if (shapes.Count == 0) return new Point(2, 3);
        return new Point(2, shapes.Max(s => s.CaveCoordinates.Y + s.Height) + 3);
    }

    internal void Print() {
        for (int y = Height - 1; y >= 0; y--) {
            Console.Write('|');
            for (int x = 0; x < width; x++) {
                var shape = FindShapeAtPosition(new Point(x, y));
                if (shape == null) {
                    Console.Write('.');
                } else {
                    Console.Write(shape.Symbol);
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

    private Shape? FindShapeAtPosition(Point point) {
        return shapes.SingleOrDefault(s => s.HitGlobal(point));
    }

    private bool IsFree(Shape excludeShape, Point point) {
        var shapeFound = FindShapeAtPosition(point);
        if (shapeFound == null) return true;
        if (shapeFound == excludeShape) return true;
        return false;
    }
}