using System.Drawing;
using utils;

using var r = new MyReader(File.OpenText("input.txt"));

var head = new Point(0, 0);
var tail = new Point(0, 0);
var tailPositions = new HashSet<Point>();
tailPositions.Add(tail);

while (!r.EOF) {
    var direction = r.ReadWord();
    var length = r.ReadInt();

    for (int i = 0; i < length; i++) {
        MoveHead(ref head, direction);
        AdaptTail(head, ref tail);
        tailPositions.Add(tail);
    }
}

Console.WriteLine("part 1: " + tailPositions.Count());

void MoveHead(ref Point head, string direction) {
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

void AdaptTail(Point head, ref Point tail) {
    if (tail.X < head.X - 1) {
        tail.X++;
        tail.Y = head.Y;
    }
    if (tail.X > head.X + 1) {
        tail.X--;
        tail.Y = head.Y;
    }
    if (tail.Y < head.Y - 1) {
        tail.Y++;
        tail.X = head.X;
    }
    if (tail.Y > head.Y + 1) {
        tail.Y--;
        tail.X = head.X;
    }
}
