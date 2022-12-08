﻿using utils;

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
        bool isVisible = IsVisible(forrest, row, col);
        if (isVisible) visibleTrees++;
        int treesInSight = CountVisibleTrees(forrest, row, col);
        maxTreesInSight = Math.Max(maxTreesInSight, treesInSight);
        Console.WriteLine($"row={row},col={col}: {forrest[row][col]}, isVisible: {isVisible}, treesInSight: {treesInSight}");
    }
}

Console.WriteLine("part 1: " + visibleTrees);
Console.WriteLine("part 2: " + maxTreesInSight);

static bool IsVisible(List<List<int>> forrest, int row, int col) {
    bool left, right, top, bottom;
    left = right = top = bottom = true;
    
    // visible from left (search columns leftward)
    for (int compareCol = col - 1; compareCol >= 0; compareCol--) {
        if (forrest[row][col] <= forrest[row][compareCol]) left = false;
    }

    // visible from right (search columns rightward)
    for (int compareCol = col + 1; compareCol < forrest[row].Count; compareCol++) {
        if (forrest[row][col] <= forrest[row][compareCol]) right = false;
    }

    // visible from top (search row upward)
    for (int compareRow = row - 1; compareRow >= 0; compareRow--) {
        if (forrest[row][col] <= forrest[compareRow][col]) top = false;
    }

    // visible from bottom (search row downward)
    for (int compareRow = row + 1; compareRow < forrest.Count; compareRow++) {
        if (forrest[row][col] <= forrest[compareRow][col]) bottom = false;
    }
    return left || right || top || bottom;
}


static int CountVisibleTrees(List<List<int>> forrest, int row, int col) {
    int left, right, top, bottom;
    left = right = top = bottom = 0;

    // visible from left (search columns leftward)
    for (int compareCol = col - 1; compareCol >= 0; compareCol--) {
        if (forrest[row][col] > forrest[row][compareCol]) left++;
        if (forrest[row][col] <= forrest[row][compareCol]) { left++; break; }
    }

    // visible from right (search columns rightward)
    for (int compareCol = col + 1; compareCol < forrest[row].Count; compareCol++) {
        if (forrest[row][col] > forrest[row][compareCol]) right++;
        if (forrest[row][col] <= forrest[row][compareCol]) { right++; break; }
    }

    // visible from top (search row upward)
    for (int compareRow = row - 1; compareRow >= 0; compareRow--) {
        if (forrest[row][col] > forrest[compareRow][col]) top++;
        if (forrest[row][col] <= forrest[compareRow][col]) { top++; break; }
    }

    // visible from bottom (search row downward)
    for (int compareRow = row + 1; compareRow < forrest.Count; compareRow++) {
        if (forrest[row][col] > forrest[compareRow][col]) bottom++;
        if (forrest[row][col] <= forrest[compareRow][col]) { bottom++; break; }
    }
    return left * right * top * bottom;
}