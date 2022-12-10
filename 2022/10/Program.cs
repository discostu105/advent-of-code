using utils;

using var r = new MyReader(File.OpenText("input.txt"));

int cycle = 0;
int x = 1;
int signalStrengthSum = 0;
while (!r.EOF) {
    var instr = r.ReadWord();
    if (instr == "noop") {
        cycle++;
        signalStrengthSum += CheckSignalStrength(cycle, x);
    } else if (instr == "addx") {
        var num = r.ReadInt();
        cycle++;
        signalStrengthSum += CheckSignalStrength(cycle, x);
        cycle++;
        signalStrengthSum += CheckSignalStrength(cycle, x);
        x += num;
    }
}

Console.WriteLine("X: " + x);
Console.WriteLine("signal strength sum: " + signalStrengthSum);

static int CheckSignalStrength(int cycle, int x) {
    if (cycle == 20 ||
        cycle == 60 ||
        cycle == 100 ||
        cycle == 140 ||
        cycle == 180 ||
        cycle == 220) {
        Console.WriteLine("cycle: " + cycle + ", x: " + x + ", signalstr: " + cycle * x);
        return cycle * x;
    }
    return 0;
}