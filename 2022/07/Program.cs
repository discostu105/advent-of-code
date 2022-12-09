using utils;

using var r = new MyReader(File.OpenText("input.txt"));

var dirsizes = new Dictionary<string, int>();
var dirstack = new Stack<string>();

string curdir = "";
string listdir = "";
while (!r.EOF) {
    var line = r.ReadLine();
    if (line.StartsWith("$")) listdir = "";

    if (line == "$ cd ..") {
        curdir = ExitDir(dirstack);
    } else if (line.StartsWith("$ cd")) {
        var dir = line.Split(' ')[2];
        curdir = EnterDir(dir, curdir, dirstack);
        if (!dirsizes.ContainsKey(curdir)) dirsizes.Add(curdir, 0);
    } else if (line.StartsWith("dir ")) {
        if (listdir != "") {
            dirsizes.Add(listdir + line.Split(' ')[1] + "/", 0);
        }
    } else if (char.IsDigit(line[0])) {
        if (listdir != "") {
            foreach (var key in dirsizes.Keys) {
                if (listdir.StartsWith(key)) dirsizes[key] += int.Parse(line.Split(' ')[0]);
            }
        }
    } else if (line == "$ ls") {
        listdir = curdir;
    }
}

int sum = 0;
foreach (var item in dirsizes) {
    Console.WriteLine(item);
    if (item.Value <= 100_000) sum += item.Value;
}
Console.WriteLine("part 1: " + sum);

int todelete = 30000000 - (70000000 - dirsizes["/"]);
Console.WriteLine(todelete);
var x = dirsizes.Where(kvp => kvp.Value > todelete).OrderBy(kvp => kvp.Value).First();
Console.WriteLine(x);

string ExitDir(Stack<string> dirstack) {
    dirstack.Pop();
    return dirstack.Peek();
}

string EnterDir(string dir, string curdir, Stack<string> dirstack) {
    var newdir = dir.StartsWith("/") ? dir : curdir + dir + "/";
    dirstack.Push(newdir);
    return newdir;
}