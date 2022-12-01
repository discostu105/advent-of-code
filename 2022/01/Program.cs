using utils;

var r = new MyReader(File.OpenText("small.txt"));

var maxCalories = 0;
while (!r.EOF)
{
    var calories = 0;
    while (!r.EOL)
    {
        calories += r.ReadInt();
    }
    r.SkipEOL();

    Console.WriteLine("new elf: " + calories);
    maxCalories = Math.Max(maxCalories, calories);
}

Console.WriteLine("max calories: " + maxCalories);
