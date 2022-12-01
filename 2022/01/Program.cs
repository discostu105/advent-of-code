using System.Linq;
using utils;

var r = new MyReader(File.OpenText("small.txt"));

var maxCalories = 0;
var elfes = new SortedList<int, int>();
while (!r.EOF)
{
    var calories = 0;
    while (!r.EOL)
    {
        calories += r.ReadInt();
    }
    r.SkipEOL();
    elfes.Add(calories, calories);
    maxCalories = Math.Max(maxCalories, calories);
}

Console.WriteLine("max calories: " + (elfes.TakeLast(3).Sum(x => x.Value)));
