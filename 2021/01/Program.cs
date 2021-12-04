var lines = File.ReadAllLines("input.txt");

int inc = 0;

for(int i=1; i<lines.Length; i++) {
    if (int.TryParse(lines[i-1], out var num0) && int.TryParse(lines[i], out var num1)) {
        if (num1 > num0) inc++;
    }
}

Console.WriteLine(inc);
