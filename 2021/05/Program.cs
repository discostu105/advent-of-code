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

var dangerous = grid.Cast<int>().Count(x => x >= 2);

//PrintGrid(grid);

Console.WriteLine($"dangerous: {dangerous}");
// wrong: 19830 -- too low

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

    override public string ToString() {
        return $"{X},{Y}";
    }
}

class Vector : IParsable {
    public Point A { get; set; }
    public Point B { get; set; }

    public void Parse(MyReader r) {
        A = r.ReadObject<Point>();
        r.Skip("->");
        B = r.ReadObject<Point>();
    }

    internal bool IsHorizontal() {
        return A.X == B.X;
    }

    internal bool IsVertical() {
        return A.Y == B.Y;
    }
    internal bool IsDiagonal() {
        return !IsVertical() && !IsHorizontal();
    }

    public override string ToString() {
        return $"{A} -> {B}";
    }

    internal void MarkGrid(int[,] grid) {
        if (IsHorizontal()) MarkGridHorizontal(grid);
        if (IsVertical()) MarkGridVertical(grid);
        if (IsDiagonal()) MarkGridDiagonal(grid);

    }
    private void MarkGridDiagonal(int[,] grid) {
        if (!IsDiagonal()) return;
        bool xrising = A.X < B.X;
        bool yrising = A.Y < B.Y;

        int x = A.X;
        int y = A.Y;
        while(true) {
            bool lastloop = x == B.X || y == B.Y;
            grid[y, x]++;
            if (xrising) x++; else x--;
            if (yrising) y++; else y--;
            if (lastloop) break;
        }
    }

    private void MarkGridHorizontal(int[,] grid) {
        if (!IsHorizontal()) return;
        int x = A.X;
        int lower;
        int upper;
        if (A.Y > B.Y) {
            lower = B.Y;
            upper = A.Y;
        } else {
            lower = A.Y;
            upper = B.Y;
        }
        for (int y = lower; y <= upper; y++) {
            grid[y, x]++;
        }
    }
    private void MarkGridVertical(int[,] grid) {
        if (!IsVertical()) return;
        int y = A.Y;
        int lower;
        int upper;
        if (A.X > B.X) {
            lower = B.X;
            upper = A.X;
        } else {
            lower = A.X;
            upper = B.X;
        }
        for (int x = lower; x <= upper; x++) {
            grid[y, x]++;
        }
    }
}