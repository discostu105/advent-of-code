using utils;

var r = new MyReader(File.OpenText("large.txt"));
using var stopwatch = AutoStopwatch.Start();

var positions = new List<int>();
while (!r.EOF) positions.Add(r.ReadInt());

var minDistance = int.MaxValue;
var idealPos = -1;
for (int pos = 0; pos < positions.Count; pos++) {
    var distance = positions.Sum(p => FuelCost(p, pos));
    if (distance < minDistance) {
        minDistance = distance;
        idealPos = pos;
    }
}

Console.WriteLine($"idealPos: {idealPos}");
Console.WriteLine($"minDistance: {minDistance}");

int FuelCost(int p, int pos) {
    var distance = Math.Abs(p - pos);
    // part1
    // return distance;

    // part2
    int sum = 0;
    for (int i = 0; i < distance; i++) {
        sum += i + 1;
    }
    return sum;
}
