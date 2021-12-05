using utils;

var r = new MyReader(File.OpenText("large.txt"));
using var stopwatch = AutoStopwatch.Start();

var vectors = new List<Vector>();
int maxx = 0;
int maxy = 0;
while (!r.EOF) {
    var vector = r.ReadObject<Vector>();
    vectors.Add(vector);
    maxx = Math.Max(maxx, vector.A.X);
    maxx = Math.Max(maxx, vector.B.X);
    maxy = Math.Max(maxy, vector.A.Y);
    maxy = Math.Max(maxy, vector.B.Y);
}

var grid = new int[maxy + 1, maxx + 1];

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

    public override string ToString() => $"{A} -> {B}";

    internal void MarkGrid(int[,] grid) {
        bool xrising = A.X < B.X;
        bool yrising = A.Y < B.Y;
        bool xflat = A.X == B.X;
        bool yflat = A.Y == B.Y;

        int x = A.X;
        int y = A.Y;
        while(true) {
            bool lastloop = x == B.X && y == B.Y;
            grid[y, x]++;
            if (!xflat) {
                if (xrising) x++; else x--;
            }
            if (!yflat) {
                if (yrising) y++; else y--;
            }
            if (lastloop) break;
        }
    }
}