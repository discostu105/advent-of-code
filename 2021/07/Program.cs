using utils;

var r = new MyReader(File.OpenText("large.txt"));
using var stopwatch = AutoStopwatch.Start();

var positions = new List<int>();
while (!r.EOF) positions.Add(r.ReadInt());

var minDistance = int.MaxValue;
for (int pos = 0; pos < positions.Count; pos++) {
    var distance = positions.Sum(p => FuelCost(p, pos, true));
    if (distance < minDistance) minDistance = distance;
}

Console.WriteLine($"minDistance: {minDistance}");

int FuelCost(int p, int pos, bool constFuel) {
    var distance = Math.Abs(p - pos);
    if (constFuel) return distance;
    return (distance * (distance + 1)) / 2;
}