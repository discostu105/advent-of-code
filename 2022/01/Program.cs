using utils;

var r = new MyReader(File.OpenText("small.txt"));

var elves = new SortedList<int, int>();
while (!r.EOF)
{
    var calories = 0;
    while (!r.EOL)
    {
        calories += r.ReadInt();
    }
    r.SkipEOL();
    elves.Add(calories, calories);
}

Console.WriteLine("top 1 elve calories: " + (elves.TakeLast(1).Sum(x => x.Value)));
Console.WriteLine("top 3 elve calories: " + (elves.TakeLast(3).Sum(x => x.Value)));
