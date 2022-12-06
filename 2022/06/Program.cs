using utils;

using var r = new MyReader(File.OpenText("input.txt"));

while (!r.EOF) {
    var line = r.ReadLine();
	var dupIdx = DuplicateIndex(line, 4);
	Console.WriteLine(dupIdx);
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
		if (Contains(s, s[i], i)) return true;
	}
	return false;
}

bool Contains(string s, char c, int except) {
	for (int i = 0; i < s.Length; i++) {
		if (i == except) continue;
		if (s[i] == c) return true;
	}
	return false;
}