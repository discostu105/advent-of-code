using utils;

var r = new MyReader(File.OpenText("small.txt"));

// minimal allocations solution
// just remember top N values
// keep a sum. if a value enters top N, then adapt the sum (subtract removed value, add new value)

var n = 3;
var topNCalories = new int[n];
int smallestIdx = 0;
while (!r.EOF)
{
    var calories = 0;
    while (!r.EOL)
    {
        calories += r.ReadInt();
    }
    r.SkipEOL();

    var currentSmallest = topNCalories[smallestIdx];
    if (currentSmallest < calories) {
        topNCalories[smallestIdx] = calories;
        smallestIdx = FindSmallestIdx(topNCalories);
    }
}

int FindSmallestIdx(int[] elves) {
    var idx = -1;
    var smallestVal = int.MaxValue;
    for (int i = 0; i < elves.Length; i++) {
        if (elves[i] < smallestVal) {
            idx = i;
            smallestVal = elves[i];
        }
    }
    return idx;
}

Console.WriteLine($"top {n} elve calories: " + topNCalories.Sum());

/*
top 1 elve calories: 75622
top 3 elve calories: 213159
*/