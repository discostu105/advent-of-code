using utils;

using var r = new MyReader(File.OpenText("input.txt"));

int stackcount = -1;
Stack<char>[] stacks = null;
var commands = new List<Command>();

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
        commands.Add(new Command(from, to, count));
    }
}

// reverse all
for (int i = 0; i < stackcount; i++) {
    stacks[i] = ReverseStack(stacks[i]);
}

RunCrane9001(commands, stacks);

// print result
foreach (var stack in stacks) {
    Console.Write(stack.Peek());
}

// part 1
static void RunCrane9000(List<Command> commands, Stack<char>[] stacks) {
    foreach (var cmd in commands) {
        for (int i = 0; i < cmd.count; i++) {
            var crate = stacks[cmd.from - 1].Pop();
            stacks[cmd.to - 1].Push(crate); // from part 1)
        }
    }
}

// part 2
static void RunCrane9001(List<Command> commands, Stack<char>[] stacks) {
    foreach (var cmd in commands) {
        var temp = new Stack<char>();
        for (int i = 0; i < cmd.count; i++) {
            var crate = stacks[cmd.from - 1].Pop();
            temp.Push(crate);
        }
        while (temp.Count > 0) {
            stacks[cmd.to - 1].Push(temp.Pop());
        }
    }
}

static Stack<char> ReverseStack(Stack<char> input) {
    var temp = new Stack<char>();
    while (input.Count != 0) temp.Push(input.Pop());
    return temp;
}

record Command(int from, int to, int count);