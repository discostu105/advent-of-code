using utils;

var r = new MyReader(File.OpenText("large.txt"));
using var stopwatch = AutoStopwatch.Start();

var vectors = new List<Vector>();
while (!r.EOF) {
    var vector = r.ReadObject<Vector>();
    vectors.Add(vector);
}

var grid = new int[vectors.Max(v => v.MaxY) + 1, vectors.Max(v => v.MaxX) + 1];

foreach (var vector in vectors) {
    vector.MarkGrid(grid);
}

if (grid.GetLength(0) < 30) PrintGrid(grid); // only print small grids

var dangerous = grid.Cast<int>().Count(x => x >= 2);
Console.WriteLine($"dangerous: {dangerous}");

void PrintGrid(int[,] grid) {
    for (int y = 0; y < grid.GetLength(0); y++) {
        for (int x = 0; x < grid.GetLength(1); x++) {
            var val = grid[y, x];
            if (val == 0) Console.Write(".");
            else Console.Write(val);
        }
        Console.WriteLine();
    }
}

class Point : IParsable {
    public int X { get; set; }
    public int Y { get; set; }
    public void Parse(MyReader r) {
        X = r.ReadInt();
        Y = r.ReadInt();
    }
    override public string ToString() => $"{X},{Y}";
}

class Vector : IParsable {
    public Point A { get; set; }
    public Point B { get; set; }

    public void Parse(MyReader r) {
        A = r.ReadObject<Point>();
        r.Skip("->");
        B = r.ReadObject<Point>();
    }

    internal bool IsHorizontal() => A.X == B.X;
    internal bool IsVertical() => A.Y == B.Y;
    internal bool IsDiagonal() => !IsVertical() && !IsHorizontal();
    internal int MaxX => Math.Max(A.X, B.X);
    internal int MaxY => Math.Max(A.X, B.X);

    public override string ToString() => $"{A} -> {B}";

    internal void MarkGrid(int[,] grid) {
        int steps = Math.Max(Math.Abs(B.X - A.X), Math.Abs(B.Y - A.Y));
        int xinc = (B.X - A.X) / steps;
        int yinc = (B.Y - A.Y) / steps;

        int x = A.X - xinc;
        int y = A.Y - yinc;
        do {
            x += xinc;
            y += yinc;
            grid[y, x]++;
        } while (x != B.X || y != B.Y);
    }
}