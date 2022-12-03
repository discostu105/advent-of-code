using utils;

// part 1
{
    using var r = new MyReader(File.OpenText("test.txt"));
    var sum = 0;
    while (!r.EOF) {
        string line = r.ReadLine();
        int compartmentSize = line.Length / 2;
        var compartment1 = line.Substring(0, compartmentSize);
        var compartment2 = line.Substring(compartmentSize);

        char match = '_';
        foreach (var c in compartment1) {
            if (compartment2.Contains(c)) match = c;
        }
        sum += Priority(match);
    }
    Console.WriteLine("part 1 solution:" + sum);
}

// part 2
{
    using var r = new MyReader(File.OpenText("input.txt"));

    var n = 3;
    var groups = new string[n];
    int sum = 0;
    while (!r.EOF) {
        for (int i = 0; i < n; i++) groups[i] = r.ReadLine();
        char badge = '_';
        foreach(var c in groups[0]) {
            bool match = true;
            for (int i = 1; i < n; i++) {
                if (!groups[i].Contains(c)) match = false;
            }
            if (match) badge = c;
        }
        sum += Priority(badge);
    }
    Console.WriteLine("part 1 solution: " + sum);
}

int Priority(char c) {
    if (c >= 'A' && c <= 'Z') return (int)c - (int)'A' + 27;
    if (c >= 'a' && c <= 'z') return (int)c - (int)'a' + 1;
    throw new Exception("invalid char: " + c);
}