using System.Diagnostics;
using utils;

using var r = new MyReader(File.OpenText("input.txt"));

var forrest = new List<List<int>>();

while (!r.EOF) {
    var line = r.ReadLine();
    var trees = new List<int>();
    foreach (var c in line) trees.Add(c - '0');
    forrest.Add(trees);
}

int visibleTrees = 0;
int maxTreesInSight = 0;
for (int row = 0; row < forrest.Count; row++) {
    for (int col = 0; col < forrest[row].Count; col++) {
        CountVisibleTrees(forrest, row, col, out var isVisibleFromOutside, out var treesInSight);
        if (isVisibleFromOutside) visibleTrees++;
        maxTreesInSight = Math.Max(maxTreesInSight, treesInSight);
        Console.WriteLine($"row={row},col={col}: {forrest[row][col]}, isVisibleFromOutside: {isVisibleFromOutside}, treesInSight: {treesInSight}");
    }
}

Console.WriteLine("part 1: " + visibleTrees);
Console.WriteLine("part 2: " + maxTreesInSight);

static void CountVisibleTrees(List<List<int>> forrest, int row, int col, out bool isVisibleFromOutside, out int treesInSight) {
    int left, right, top, bottom;
    left = right = top = bottom = 0;

    bool vleft, vright, vtop, vbottom;
    vleft = vright = vtop = vbottom = true;

    // visible from left (search columns leftward)
    for (int compareCol = col - 1; compareCol >= 0; compareCol--) {
        if (forrest[row][col] > forrest[row][compareCol]) left++;
        if (forrest[row][col] <= forrest[row][compareCol]) { left++; vleft &= false; break; }
    }

    // visible from right (search columns rightward)
    for (int compareCol = col + 1; compareCol < forrest[row].Count; compareCol++) {
        if (forrest[row][col] > forrest[row][compareCol]) right++;
        if (forrest[row][col] <= forrest[row][compareCol]) { right++; vright &= false; break; }
    }

    // visible from top (search row upward)
    for (int compareRow = row - 1; compareRow >= 0; compareRow--) {
        if (forrest[row][col] > forrest[compareRow][col]) top++;
        if (forrest[row][col] <= forrest[compareRow][col]) { top++; vtop &= false; break; }
    }

    // visible from bottom (search row downward)
    for (int compareRow = row + 1; compareRow < forrest.Count; compareRow++) {
        if (forrest[row][col] > forrest[compareRow][col]) bottom++;
        if (forrest[row][col] <= forrest[compareRow][col]) { bottom++; vbottom &= false; break; }
    }
    isVisibleFromOutside = vleft || vright || vtop || vbottom;
    treesInSight = left * right * top * bottom;
}