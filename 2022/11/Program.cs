using utils;

using var r = new MyReader(File.OpenText("input.txt"));

var monkeys = new List<Monkey>();
while (!r.EOF) {
    var line = r.ReadLine();
    if (line.StartsWith("Monkey")) {
        using var linereader = MyReader.FromString(line, new char[] { ' ', ':' });
        linereader.Skip("Monkey");
        int monkeyIndex = linereader.ReadInt();
        line = r.ReadLine();
        var startingItems = line.Substring("  Starting items:".Length).Replace(" ", "").Split(',').Select(x => long.Parse(x)).ToList();

        var operation = Operation.Parse(r.ReadLine().Substring("  Operation: new = ".Length));
        int testDivisibleBy = int.Parse(r.ReadLine().Substring("  Test: divisible by ".Length));
        int trueMonkeyIdx = int.Parse(r.ReadLine().Substring("    If true: throw to monkey ".Length));
        int falseMonkeyIdx = int.Parse(r.ReadLine().Substring("    If false: throw to monkey ".Length));
        monkeys.Add(new Monkey(monkeyIndex, operation, testDivisibleBy, trueMonkeyIdx, falseMonkeyIdx) { Items = startingItems });
    }
}

PrintMonkeyItems(monkeys);
var leastCommonMultiple = FindLeastCommonMultiple(monkeys.Select(m => m.TestDivisibleBy).ToList());
var inspectionCounts = PlayRounds(monkeys, 10000, 1, leastCommonMultiple).OrderByDescending(x => x).ToList();
Console.WriteLine("lcm: " + leastCommonMultiple);
Console.WriteLine(inspectionCounts[0]);
Console.WriteLine(inspectionCounts[1]);
Console.WriteLine(inspectionCounts[0] * inspectionCounts[1]);

long[] PlayRounds(List<Monkey> monkeys, int rounds, long worryDeclineRate, int leastCommonMultiple) {
    var inspectionCounts = new long[monkeys.Count];
    for (int i = 0; i < rounds; i++) {
        // Console.WriteLine("Round " + i);
        foreach (var monkey in monkeys) {
            var itemCopy = new List<long>(monkey.Items);
            monkey.Items = new List<long>(); // empty current item list
            foreach (var item in itemCopy) {
                var newItem = monkey.Operation.Calc(item) / worryDeclineRate;

                newItem %= leastCommonMultiple;

                if (newItem % monkey.TestDivisibleBy == 0) {
                    monkeys[monkey.TrueMonkeyIdx].Items.Add(newItem);
                } else {
                    monkeys[monkey.FalseMonkeyIdx].Items.Add(newItem);
                }
                inspectionCounts[monkey.Index]++;
            }

        }

        // PrintMonkeyItems(monkeys);
    }
    return inspectionCounts;
}

static void PrintMonkeyItems(List<Monkey> monkeys) {
    foreach (var monkey in monkeys) {
        Console.WriteLine($"Monkey {monkey.Index}: " + string.Join(", ", monkey.Items));
    }
    Console.WriteLine();
}

int FindLeastCommonMultiple(List<int> list) {
    return LcmOfArray(list, 0);
}

static int gcd(int a, int b) {
    if (a == 0)
        return b;
    return gcd(b % a, a);
}

static int LcmOfArray(List<int> arr, int idx) {
    if (idx == arr.Count - 1) {
        return arr[idx];
    }
    int a = arr[idx];
    int b = LcmOfArray(arr, idx + 1);
    return (a * b / gcd(a, b));
}

record class Monkey(int Index, Operation Operation, int TestDivisibleBy, int TrueMonkeyIdx, int FalseMonkeyIdx) {
    public List<long> Items { get; set; }
}
record Operation(string left, string operand, string right) {
    public long Calc(long old) {
        var leftVal = left == "old" ? old : int.Parse(left);
        var rightVal = right == "old" ? old : int.Parse(right);
        if (operand == "+") return leftVal + rightVal;
        if (operand == "-") return leftVal - rightVal;
        if (operand == "*") return leftVal * rightVal;
        if (operand == "%") return leftVal / rightVal;
        throw new ArgumentException();
    }

    public static Operation Parse(string str) {
        var parts = str.Split(' ');
        return new Operation(parts[0], parts[1], parts[2]);
    }
}
