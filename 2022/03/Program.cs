using utils;

// part 1
{
    int sum = 0;
    using var r = new MyReader(File.OpenText("input.txt"));
    while (!r.EOF) {
        var line = r.ReadLine();
        sum += Priority(FindCommonCharacter(new string[] { 
            line[..(line.Length / 2)], 
            line[(line.Length / 2)..]}
        ));
    }
    Console.WriteLine("part 1 solution: " + sum);
}

// part 2
{
    var n = 3;
    var groups = new string[n];
    int sum = 0;

    using var r = new MyReader(File.OpenText("input.txt"));
    while (!r.EOF) {
        for (int i = 0; i < n; i++) groups[i] = r.ReadLine();
        sum += Priority(FindCommonCharacter(groups));
    }
    Console.WriteLine("part 2 solution: " + sum);
}

static char FindCommonCharacter(string[] groups) {
    char badge = default(char);
    foreach (var c in groups[0]) {
        bool match = true;
        for (int i = 1; i < groups.Length; i++) {
            if (!groups[i].Contains(c)) match = false;
        }
        if (match && badge != c) {
            if (badge != default(char)) throw new Exception("multiple common characters found");
            badge = c;
        }
    }
    if (badge == default(char)) throw new Exception("no common character found");
    return badge;
}

int Priority(char c) {
    if (c >= 'A' && c <= 'Z') return (int)c - (int)'A' + 27;
    if (c >= 'a' && c <= 'z') return (int)c - (int)'a' + 1;
    throw new Exception("invalid char: " + c);
}