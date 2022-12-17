using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using utils;

using var r = new MyReader(File.OpenText("input.txt"));

while (!r.EOF) {

}

var cave = new Cave(7);

var shape = Shape.NewPlus();
cave.SpawnShape(shape);

cave.TryPush(shape, '>');
cave.TryFall(shape);
cave.Print();

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

    public Shape(char symbol, string[] sprite) {
        Symbol = symbol;
        Sprite = sprite;
    }

    internal static Shape NewPlus() => new Shape('+', new[] {
        ".#.",
        "###",
        ".#.",
    });

    internal static Shape NewDash() => new Shape('+', new[] {
        "####",
    });

    internal static Shape NewL() => new Shape('+', new[] {
        "..#",
        "..#",
        "###",
    });

    internal static Shape NewI() => new Shape('+', new[] {
        "#",
        "#",
        "#",
        "#",
    });

    internal static Shape NewSquare() => new Shape('+', new[] {
        "##",
        "##",
    });

    public bool HitLocal(Point coord) {
        return Sprite[coord.Y][coord.X] == '#';
    }

    public bool HitGlobal(Point coord) {
        return HitLocal(new Point(coord.X - CaveCoordinates.X, coord.Y - CaveCoordinates.Y);
    }
}

class Cave {
    private int width;
    private List<Shape> shapes = new List<Shape>();

    public int Height { get => shapes.Max(s => s.CaveCoordinates.X); }

    public Cave(int width) {
        this.width = width;
    }

    internal void SpawnShape(Shape shape) {
        shape.CaveCoordinates = FindSpawnPosition(shape);
        shapes.Add(shape);
    }

    internal bool TryFall(Shape shape) {
        var fallCoordinates = new Point(shape.CaveCoordinates.X, shape.CaveCoordinates.Y - 1);
        bool canFall = TestPosition(shape, fallCoordinates);
        if (!canFall) return false;
        ApplyShapePosition(shape, fallCoordinates);
        return true;
    }

    private bool TestPosition(Shape shape, Point fallCoordinates) {
        return true; // todo
    }

    internal bool TryPush(Shape shape, char direction) {
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
        return new Point(3, 3); // todo
    }

    internal void Print() {
        for (int y = Height - 1; y >= 0; y--) {
            for (int x = 0; x < width; x++) {
                var shape = FindShapeAtPosition(new Point(x, y));
                if (shape == null) {
                    Console.Write('.');
                } else {
                    Console.Write(shape.Symbol);
                }
            }
        }
    }

    private Shape? FindShapeAtPosition(Point point) {
        return shapes.SingleOrDefault(s => s.HitGlobal(point));
    }
}