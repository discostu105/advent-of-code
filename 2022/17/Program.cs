using System.Drawing;

var input = File.ReadAllText("input.txt");
var maxMoves = input.Length;
var cave = new Cave(7);

var n = 2022;
var movePos = 0;
var shapes = EnumerateShapes().GetEnumerator();
for (int i = 0; i < n; i++) {
    shapes.MoveNext();
    var shape = shapes.Current;
    cave.SpawnShape(shape);
    //cave.Print();
    while (true) {
        cave.TryPush(shape, input[movePos++]);
        movePos %= maxMoves;
        //cave.Print();
        if (!cave.TryFall(shape)) break;
        //cave.Print();
    }
}
cave.Print();
Console.WriteLine(cave.Height);

//var shape = Shape.NewDash();
//cave.SpawnShape(shape);
//cave.Print();

//cave.TryPush(shape, '<');
//cave.Print();

//cave.TryFall(shape);
//cave.Print();

//cave.TryPush(shape, '<');
//cave.Print();
//cave.TryPush(shape, '<');
//cave.Print();

//cave.TryFall(shape);
//cave.Print();

//cave.TryFall(shape);
//cave.Print();

//cave.TryFall(shape);
//cave.Print();


//var shape2 = Shape.NewSquare();
//cave.SpawnShape(shape2);
//cave.Print();

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
    public string[] Sprite { get; init; }
    public int Width { get => Sprite[0].Length; }
    public int Height { get => Sprite.Length; }

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
        return Sprite[coord.Y][coord.X] == '#';
    }

    public bool HitGlobal(Point coord) {
        return HitLocal(new Point(coord.X - CaveCoordinates.X, coord.Y - CaveCoordinates.Y));
    }
}

class Cave {
    private int width;
    private List<Shape> shapes = new List<Shape>();

    public int Height { get => shapes.Max(s => s.CaveCoordinates.Y + s.Height); }

    public Cave(int width) {
        this.width = width;
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
        if (fallCoordinates.X < 0 || fallCoordinates.X + shape.Width > width) return false;
        if (fallCoordinates.Y < 0) return false;
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