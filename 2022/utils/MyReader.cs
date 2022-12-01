using System.Text;

namespace utils;

public class MyReader {
	private StreamReader sr;

	public MyReader(StreamReader sr, char[] separators = null) {
		this.sr = sr;
		if (separators != null)
			this.Separators = separators;
		else
			this.Separators = new char[] { ' ', ',' };
	}

	public string ReadWord() {
		SkipSeparators();
		eol = false;
		var sb = new StringBuilder();
		char c;
		while (!sr.EndOfStream) {
			c = (char)sr.Read();
			if (Separators.Contains(c)) break;
			if (IsNewLine(c)) {
				eol = true;
				//char nextch = (char)sr.Peek();
				//while (IsNewLine(nextch)) {
				//	sr.Read(); // consume all newlines
				//	nextch = (char)sr.Peek();
				//}
				SkipEOL(); // skip 1x newline
				break;
			}
			sb.Append(c);
		}
		return sb.ToString();
	}

	// skip over some text. do not include separators in word
    public void Skip(string word) {
		SkipSeparators();
		int i = 0;
		while (!sr.EndOfStream) {
			sr.Read();
			i++;
			if (i == word.Length) return;
		}
	}

    private void SkipSeparators() {
		char nextch = (char)sr.Peek();
		while (Separators.Contains(nextch)) {
			sr.Read(); // consume all separators
			nextch = (char)sr.Peek();
		}
	}

    private bool IsNewLine(char c) {
		return c == '\r' || c == '\n';
	}

	public int ReadInt() {
		string w = ReadWord();
		return int.Parse(w);
	}

	public double ReadDouble() {
		return double.Parse(ReadWord());
	}

	public string ReadLine() {
		eol = false;
		var sb = new StringBuilder();
		char c;
		while (!sr.EndOfStream) {
			c = (char)sr.Read();
			if (IsNewLine(c)) {
				eol = true;
				char nextch = (char)sr.Peek();
				while (IsNewLine(nextch)) {
					sr.Read(); // consume all newlines
					nextch = (char)sr.Peek();
				}
				break;
			}
			sb.Append(c);
		}
		return sb.ToString();
	}

	public bool EOF {
		get { return sr.EndOfStream; }
	}

	// skips end-of-line characters, once. considers CRLF and LF eols.
	public void SkipEOL()
	{
        if (EOL)
		{
            char nextch = (char)sr.Peek();
            if (nextch == '\r')
            {
                sr.Read();
                nextch = (char)sr.Peek();
            }
            if (nextch == '\n')
            {
                sr.Read();
                nextch = (char)sr.Peek();
            }
            eol = (nextch == '\r' || nextch == '\n'); // it's ok if next line is EOL again
        }
	}


    public char[] Separators { get; set; }

	bool eol;
	public bool EOL {
		get { return eol || sr.EndOfStream; }
	}

	public T ReadObject<T>() where T : IParsable, new() {
		var obj = new T();
		obj.Parse(this);
		return obj;
	}

	public int[] ReadIntArray() {
		int size = ReadInt();
		var a = new int[size];
		for (int i = 0; i < size; i++) {
			a[i] = ReadInt();
		}
		return a;
	}

	public double[] ReadDoubleArray() {
		int size = ReadInt();
		var a = new double[size];
		for (int i = 0; i < size; i++) {
			a[i] = ReadDouble();
		}
		return a;
	}

	public string[] ReadWordArray() {
		int size = ReadInt();
		var a = new string[size];
		for (int i = 0; i < size; i++) {
			a[i] = ReadWord();
		}
		return a;
	}

	public T[] ReadObjectArray<T>() where T : IParsable, new() {
		int size = ReadInt();
		var a = new T[size];
		for (int i = 0; i < size; i++) {
			a[i] = ReadObject<T>();
		}
		return a;
	}

	internal void NextLine() {
		eol = false;
	}
}
public interface IParsable {
	void Parse(MyReader r);
}
