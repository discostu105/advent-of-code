using utils;

var lines = File.ReadAllLines("large.txt");

int bitcount = lines[0].Length;
var sums = CalcSums(lines, bitcount);

int gamma = 0;
int epsilon = 0;

for (int i = 0; i < bitcount; i++) {
    if (MostCommon(sums[i], lines.Length) == 1) {
        gamma |= (1 << (bitcount - i - 1));
    } else {
        epsilon |= 1 << (bitcount - i - 1);
    }
}

Console.WriteLine($"result part1: {gamma * epsilon}\n");

var oxygen = CalcMetric(lines, true);
var co2 = CalcMetric(lines, false);

Console.WriteLine($"result part2: {oxygen * co2}\n");

int[] CalcSums(IEnumerable<string> lines, int bitcount) {
    var sums = new int[bitcount];
    foreach (var bits in lines) {
        for (int i = 0; i < bitcount; i++) {
            sums[i] += int.Parse(bits[i].ToString());
        }
    }
    return sums;
}

int MostCommon(int sum, int len) {
    return sum >= ((len+1) / 2) ? 1 : 0;
}

int CalcMetric(string[] lines, bool findMostCommon) {
    var candidates = lines.ToArray();
    while (candidates.Count() > 1) {
        for (int i = 0; i < bitcount; i++) {
            var sums2 = CalcSums(candidates, bitcount);
            var mostcommon = MostCommon(sums2[i], candidates.Length);
            if (findMostCommon) {
                candidates = candidates.Where(c => int.Parse(c[i].ToString()) == mostcommon).ToArray();
            } else {
                candidates = candidates.Where(c => int.Parse(c[i].ToString()) != mostcommon).ToArray();
            }
            if (candidates.Count() == 1) break;
        }
    }
    return Convert.ToInt32(candidates.First(), 2);
}