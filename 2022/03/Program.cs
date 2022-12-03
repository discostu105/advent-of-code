using utils;

var r = new MyReader(File.OpenText("input.txt"));

var sum = 0;
while(!r.EOF) {
    string line = r.ReadLine();
    int compartmentSize = line.Length / 2;
    var compartment1 = line.Substring(0, compartmentSize);
    var compartment2 = line.Substring(compartmentSize);

    var inBothCompartments = new List<char>();
    foreach(var c in compartment1) {
        if (compartment2.Contains(c) && !inBothCompartments.Contains(c)) inBothCompartments.Add(c);
    }

    Console.WriteLine(compartment1);
    Console.WriteLine(compartment2);

    inBothCompartments.ForEach(c => Console.WriteLine(c + ": " + Priority(c)));
    sum += inBothCompartments.Sum(c => Priority(c));
}
Console.WriteLine(sum);

int Priority(char c) {
    if (c >= 'A' && c <= 'Z') return (int)c - (int)'A' + 27;
    if (c >= 'a' && c <= 'z') return (int)c - (int)'a' + 1;
    throw new Exception("invalid char: " + c);
}