﻿using utils;

using var r = new MyReader(File.OpenText("input.txt"));

while (!r.EOF) {
    var line = r.ReadLine();
    Console.WriteLine("part 1: " + FindNonDuplicateIndex(line, 4));
    Console.WriteLine("part 2: " + FindNonDuplicateIndex(line, 14));
}

int FindNonDuplicateIndex(string s, int buffersize) {
    for (int i = 0; i < s.Length - buffersize; i++) {
        if (!HasDuplicateCharacters(s[i..(i + buffersize)])) return i + buffersize;
    }
    return -1;
}

bool HasDuplicateCharacters(string s) {
    for (int i = 0; i < s.Length; i++) {
        if (s[(i + 1)..].Contains(s[i])) return true;
    }
    return false;
}