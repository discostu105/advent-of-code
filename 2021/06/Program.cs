using utils;

var r = new MyReader(File.OpenText("large.txt"));
using var stopwatch = AutoStopwatch.Start();

var fish = new Dictionary<long, long>();
while (!r.EOF) AddVal(fish, r.ReadInt(), 1);

int days = 256;
for (int i = 0; i < days; i++) {
    var newfish = new Dictionary<long, long>();
    foreach (var f in fish) {
        if (f.Value == 0) continue;
        if (f.Key == 0) {
            AddVal(newfish, 6, f.Value);
            AddVal(newfish, 8, f.Value);
        } else {
            AddVal(newfish, f.Key - 1, f.Value);
        }
    }
    fish = newfish;
}

Console.WriteLine(fish.Values.Sum());

void AddVal(Dictionary<long, long> dict, long key, long val) {
    if (!dict.ContainsKey(key)) {
        dict.Add(key, val);
    } else {
        dict[key] += val;
    }
}