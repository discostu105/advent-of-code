using utils;

var r = new MyReader(File.OpenText("large.txt"));

long x = 0;
long z = 0;
long aim = 0;

while (!r.EOF) {
    var direction = r.ReadWord();
    var value = r.ReadInt();
    if (direction == "forward") {
        x += value;
        z += aim * value;
    }
    if (direction == "down") aim += value;
    if (direction == "up") aim -= value;
}

Console.WriteLine(x * z);