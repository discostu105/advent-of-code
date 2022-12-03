using System.Diagnostics;
using utils;

string file = "small.txt";

var digitDefinition = new Dictionary<int, string> {
    { 0, "abcefg" },
    { 1, "cf" },
    { 2, "acdeg" },
    { 3, "acdfg" },
    { 4, "bcdf" },
    { 5, "abdfg" },
    { 6, "abdefg" },
    { 7, "acf" },
    { 8, "abcdefg" },
    { 9, "abcdfg" },
};

Console.WriteLine("part 1:");
Solve();
//Console.WriteLine("part 2:");
//Solve();

void Solve() {
    using var stopwatch = AutoStopwatch.Start();
    using var r = new MyReader(File.OpenText(file));
    int foundUniqueDigit = 0;
    int lineCnt = 0;

    while (!r.EOF) {
        var line = r.ReadLine().Split(" | ");
        var uniqueSignals = line[0].Split(' ');
        var digitOutputs = line[1].Split(' ');

        var wiring = new Wiring(digitDefinition);

        foreach (var signal in uniqueSignals) {
            // Console.WriteLine(signal);
            wiring.MarkCandidates(signal);
        }
        //Console.WriteLine(wiring);
        wiring.PrintCandidates();

        Console.WriteLine("test:");
        foreach (var c in "dab") {
            Console.Write($"({c}: {string.Join(",", wiring.GetPossibleChars(c))}) ");
        }
        Console.WriteLine();

        Console.WriteLine(lineCnt + ": " + String.Join(",", uniqueSignals));
        foreach (var signal in uniqueSignals) {
            Console.Write($"{signal}: ");
            foreach (var c in signal) {
                Console.Write($"({c}: {string.Join(",", wiring.GetPossibleChars(c))}) ");
            }
            Console.WriteLine();
        }

        Console.WriteLine(lineCnt + ": " + String.Join(",", digitOutputs));
        foreach (var signal in digitOutputs) {
            //var digit = wiring.FindSingleDigitByLength(signal);
            //if (digit != null) {
            //    foundUniqueDigit++;
            //    Console.WriteLine($"{signal}: {digit}");
            //}
            Console.Write($"{signal}: ");
            foreach(var c in signal) {
                Console.Write($"({c}: {string.Join(",", wiring.GetPossibleChars(c))}) ");
            }
            Console.WriteLine();
        }
        lineCnt++;
    }
    Console.WriteLine(foundUniqueDigit); // wrong: 40, wrong: 800 (too high)
}

class Wiring {
    private Dictionary<char, string> _wireCandidates = new Dictionary<char, string>();
    private readonly Dictionary<int, string> _digitDefinition;

    public Wiring(Dictionary<int, string> digitDefinition) {
        this._digitDefinition = digitDefinition;

        // init with all possibilities
        for (int ch = 'a'; ch <= 'g'; ch++) {
            _wireCandidates[(char)ch] = "abcdefg";
        }
    }

    public void MarkCandidates(string pattern) {
        var charCandidateGroups = _digitDefinition.Where(x => x.Value.Length == pattern.Length).Select(x => x.Value).ToList();

        foreach(var charCandidates in charCandidateGroups) {
            var reducedWireCandidates = ReduceWireCandidates(_wireCandidates, pattern, charCandidates);
            if (IsValid(reducedWireCandidates)) _wireCandidates = reducedWireCandidates;
            Console.WriteLine("after " + pattern);
            Console.WriteLine(this);
        }
    }

    private bool IsValid(Dictionary<char, string> wireCandidates) {
        if (wireCandidates.Any(x => string.IsNullOrEmpty(x.Value))) return false;
        foreach(var c in "abcdefg") {
            if (wireCandidates.Count(x => x.Value.Length == 1 && x.Value.Contains(c)) > 1) return false;
        }
        return true;
    }

    private Dictionary<char, string> ReduceWireCandidates(Dictionary<char, string> wireCandidates, string pattern, string charCandidates) {
        var newWireCandidates = new Dictionary<char, string>(wireCandidates);
        foreach (var ch in pattern) {
            newWireCandidates[ch] = new string(charCandidates.Intersect(newWireCandidates[ch]).ToArray());
        }
        return newWireCandidates;
    }

    public IEnumerable<char> GetPossibleChars(char ch) {
        return _wireCandidates.Where(candidate => candidate.Value.Contains(ch)).Select(x => x.Key);
    }

    public int? FindSingleDigitByLength(string pattern) {
        var digitCandidates = FindDigitCandidtesByLength(pattern).ToList();
        if (digitCandidates.Count == 1) return digitCandidates[0];
        return null;
    }

    public IEnumerable<int> FindDigitCandidtesByLength(string pattern) {
        return _digitDefinition.Where(x => x.Value.Length == pattern.Length).Select(x => x.Key);
    }

    public override string ToString() {
        return String.Join('\n', _wireCandidates.Select(x => $"{x.Key}: [{string.Join(',', x.Value)}] "));
    }

    public void PrintCandidates() {
        foreach (var key in _wireCandidates.Keys) {
            Console.WriteLine($"{key}: {string.Join(',', _wireCandidates[key])}");
        }
    }
}