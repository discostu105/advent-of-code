using utils;

using var r = new MyReader(File.OpenText("input.txt"), new char[] { '[', ']', ' ' });

int stackcount = -1;
Stack<char>[] stacks = null;

while (!r.EOF) {
    var line = r.ReadLine();
    if (stackcount < 0) stackcount = (line.Length + 1) / 4;
    if (stacks == null) stacks = new Stack<char>[stackcount];
    if (line.Contains('[')) {
        // stack content
        for (int i = 0; i < stackcount; i++) {
            var crate = line[i * 4 + 1];
            if (stacks[i] == null) stacks[i] = new Stack<char>();
            if (crate != ' ') stacks[i].Push(crate);
        }
    } else if (line.StartsWith("move")) {
        // move command
        using var linereader = MyReader.FromString(line);

        linereader.ReadWord();
        var count = linereader.ReadInt();
        linereader.ReadWord();
        var from = linereader.ReadInt();
        linereader.ReadWord();
        var to = linereader.ReadInt();

        var temp = new Stack<char>();
        for (int i = 0; i < count; i++) {
            var crate = stacks[from - 1].Pop();
            // stacks[to - 1].Push(crate); // from part 1)
            temp.Push(crate);
        }
        while (temp.Count > 0) {
            stacks[to - 1].Push(temp.Pop());
        }
    } else {
        // skip

        for (int i = 0; i < stackcount; i++) {
            stacks[i] = ReverseStack(stacks[i]);
        }
    }
}

foreach (var stack in stacks) {
    Console.Write(stack.Peek());
}

static Stack<char> ReverseStack(Stack<char> input) {
    var temp = new Stack<char>();

    while (input.Count != 0)
        temp.Push(input.Pop());

    return temp;
}