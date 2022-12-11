using utils;

using var r = new MyReader(File.OpenText("input.txt"));

int cycle = 0;
int x = 1;
int signalStrengthSum = 0;
var checkpoints = new List<int> { 20, 60, 100, 140, 180, 220 };
while (!r.EOF) {
    var instr = r.ReadWord();
    if (instr == "noop") {
        signalStrengthSum += CheckSignalStrength(ref cycle, x, checkpoints);
    } else if (instr == "addx") {
        var num = r.ReadInt();
        signalStrengthSum += CheckSignalStrength(ref cycle, x, checkpoints);
        signalStrengthSum += CheckSignalStrength(ref cycle, x, checkpoints);
        x += num;
    }
}

Console.WriteLine("signal strength sum: " + signalStrengthSum);

static int CheckSignalStrength(ref int cycle, int x, List<int> checkpoints) {
    cycle++;
    WriteToCRT(cycle, x);
    if (checkpoints.Contains(cycle)) return cycle * x;
    return 0;
}

static void WriteToCRT(int cycle, int x) {
    int pos = cycle % 40;
    if (x > 0 && pos == 0) pos = 40;
    if (pos >= x && pos < x+3) {
        Console.Write("#");
    } else {
        Console.Write(".");

    }
    if (cycle % 40 == 0) Console.WriteLine();
}