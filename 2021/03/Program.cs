using utils;

var r = new MyReader(File.OpenText("large.txt"));

int len = 0;
int[] sums = null;
int lines = 0;

while (!r.EOF) {
    var bits = r.ReadWord();

    if (sums == null) {
        // lazy init with proper length
        len = bits.Length;
        sums = new int[len];
    }

    for (int i = 0; i < len; i++) {
        sums[i] += int.Parse(bits[i].ToString());
    }
    lines++;
}

int gamma = 0;
int epsilon = 0;
for (int i = 0; i < len; i++) {
    if (sums[i] > (lines / 2)) {
        gamma |= (1 << (len - i - 1));
    } else {
        epsilon |= 1 << (len - i - 1);
    }
}

Console.WriteLine($"result: {gamma * epsilon}\n");

Console.WriteLine($"gamma: {gamma}");
Console.WriteLine($"epsilon: {epsilon}");

Console.WriteLine($"gamma (binary): {Convert.ToString(gamma, toBase: 2)}");
Console.WriteLine($"epsilon (binary): {Convert.ToString(epsilon, toBase: 2)}");
