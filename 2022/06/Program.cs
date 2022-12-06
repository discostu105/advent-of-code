using utils;

using var r = new MyReader(File.OpenText("test.txt"));

while (!r.EOF) {
    var line = r.ReadLine();
	Console.WriteLine("part 1: " + DuplicateIndex(line, 4));
    Console.WriteLine("part 2: " + DuplicateIndex(line, 14));
}

int DuplicateIndex(string s, int buffersize) {
    for (int i = 0; i < s.Length - buffersize; i++) {
        bool contains = HasDuplicates(s[i..(i + buffersize)]);
        if (!contains) return i + buffersize;
    }
	return -1;
}

bool HasDuplicates(string s) {
	for (int i = 0; i < s.Length; i++) {
		for (int j = i+1; j < s.Length; j++) {
			if (s[i] == s[j]) return true;
		}
	}
	return false;
}