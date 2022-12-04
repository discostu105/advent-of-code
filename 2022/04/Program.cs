using utils;

using var r = new MyReader(File.OpenText("input.txt"), new char[] { '-', ',' });

int sum = 0;
while (!r.EOF) {
    var pair1 = new Pair {
        From = r.ReadInt(),
        To = r.ReadInt()
    };
    var pair2 = new Pair {
        From = r.ReadInt(),
        To = r.ReadInt()
    };
    if (pair1.From >= pair2.From && pair1.To <= pair2.To) {
        Console.WriteLine(pair1 + "," + pair2);
        sum++;
    } else if (pair1.From <= pair2.From && pair1.To >= pair2.To) {
        Console.WriteLine(pair1 + "," + pair2);
        sum++;
    }
}
Console.WriteLine(sum);
// 515

class Pair {
    public int From { get; set; }
    public int To { get; set; }

    public override string ToString() {
        return $"{From}-{To}";
    }
}