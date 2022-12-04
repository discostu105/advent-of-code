using utils;

using var r = new MyReader(File.OpenText("input.txt"), new char[] { '-', ',' });

int overlaps = 0;
int overlappingPairs = 0;
while (!r.EOF) {
    var pair1 = new Pair(r.ReadInt(), r.ReadInt());
    var pair2 = new Pair(r.ReadInt(), r.ReadInt());
    if (pair1.From >= pair2.From && pair1.To <= pair2.To) overlaps++;
    else if (pair1.From <= pair2.From && pair1.To >= pair2.To) overlaps++;

    var a = Enumerable.Range(pair1.From, pair1.To - pair1.From + 1).ToList();
    var b = Enumerable.Range(pair2.From, pair2.To - pair2.From + 1).ToList();
    var intersections = a.Intersect(b).Count();
    Console.WriteLine(pair1 + "," + pair2 + " :: " + intersections);
    if (intersections > 0) overlappingPairs++;
}
Console.WriteLine("part 1: " + overlaps);
Console.WriteLine("part 2: " + overlappingPairs);

record Pair(int From, int To) {
    public override string ToString() => $"{From}-{To}";
}