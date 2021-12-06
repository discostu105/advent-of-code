using utils;

// part1
{
    var r = new MyReader(File.OpenText("large.txt"));
    using var stopwatch = AutoStopwatch.Start();

    var fish = new List<int>();
    while (!r.EOF) fish.Add(r.ReadInt());

    int days = 80;
    for (int i = 0; i < days; i++) {
        var newfish = new List<int>();
        foreach (var f in fish) {
            if (f == 0) {
                newfish.Add(6);
                newfish.Add(8);
            } else {
                newfish.Add(f - 1);
            }
        }
        fish = newfish;
    }

    Console.WriteLine(fish.Count());
}