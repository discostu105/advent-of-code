using System.Diagnostics.CodeAnalysis;
using utils;

using var r = new MyReader(File.OpenText("input.txt"));

var knots = new List<Point>();
int n = 10;
for (int i = 0; i < n; i++) {
    knots.Add(new Point());
}
var tailPositions = new HashSet<Point>();
tailPositions.Add(new Point(knots.Last()));

while (!r.EOF) {
    var direction = r.ReadWord();
    var length = r.ReadInt();

    for (int i = 0; i < length; i++) {
        MoveHead(knots[0], direction);
        for (int j = 0; j < n - 1; j++) {
            AdaptTail(knots[j], knots[j + 1]);
        }
        if (!tailPositions.Contains(knots.Last())) {
            tailPositions.Add(new Point(knots.Last()));
        }
        //Print(knots.ToList());
    }

    Console.WriteLine();
}

//Print(knots);
Print(tailPositions.ToList());

void Print(List<Point> knots) {
    int min = -10;
    int max = 10;
    for (int i = min; i < max; i++) {
        for (int j = min; j < max; j++) {
            var p = new Point(j, -i);
            if (knots.Contains(p)) {
                var idx = knots.IndexOf(p);
                var ch = (idx > 9) ? "#" : idx.ToString();
                Console.Write(ch);
            } else {
                Console.Write(".");
            }
        }
        Console.WriteLine();
    }
}

Console.WriteLine("solution: " + tailPositions.Count());


void MoveHead(Point head, string direction) {
    if (direction == "R") {
        head.X++;
    } else if (direction == "L") {
        head.X--;
    } else if (direction == "U") {
        head.Y++;
    } else if (direction == "D") {
        head.Y--;
    }
}

void AdaptTail(Point head, Point tail) {
    if (tail.X < head.X - 1) {
        tail.X++;
        if (tail.Y < head.Y) tail.Y++;
        if (tail.Y > head.Y) tail.Y--;
    }
    if (tail.X > head.X + 1) {
        tail.X--;
        if (tail.Y < head.Y) tail.Y++;
        if (tail.Y > head.Y) tail.Y--;
    }
    if (tail.Y < head.Y - 1) {
        tail.Y++;
        if (tail.X < head.X) tail.X++;
        if (tail.X > head.X) tail.X--;
    }
    if (tail.Y > head.Y + 1) {
        tail.Y--;
        if (tail.X < head.X) tail.X++;
        if (tail.X > head.X) tail.X--;
    }
}

public class Point {
    public double X { get; set; } = 0;
    public double Y { get; set; } = 0;

    public Point() { }
    public Point(int x, int y) {
        X = x;
        Y = y;
    }
    public Point(Point p) {
        X = p.X;
        Y = p.Y;
    }

    public override bool Equals(object obj) {
        return this.X == ((Point)obj).X && this.Y == ((Point)obj).Y;
    }

    public override int GetHashCode() {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            // Suitable nullity checks etc, of course :)
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }
    }
}