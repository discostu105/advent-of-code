using utils;

string file = "large.txt";

Console.WriteLine("part 1:");
Crabs(constDistance: true);
Console.WriteLine("part 2:");
Crabs(constDistance: false);

void Crabs(bool constDistance) {
    var r = new MyReader(File.OpenText(file));
    using var stopwatch = AutoStopwatch.Start();

    var positions = new List<int>();
    while (!r.EOF) positions.Add(r.ReadInt());

    var minDistance = int.MaxValue;
    for (int pos = 0; pos < positions.Count; pos++) {
        var distance = positions.Sum(p => FuelCost(p, pos, constDistance));
        if (distance < minDistance) minDistance = distance;
    }

    Console.WriteLine($"minDistance: {minDistance}");

    int FuelCost(int p, int pos, bool constFuel) {
        var distance = Math.Abs(p - pos);
        if (constFuel) return distance;
        return (distance * (distance + 1)) / 2;
    }
}