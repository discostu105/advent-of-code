using System.ComponentModel;
using System.Drawing;
using utils;

using var r = new MyReader(File.OpenText("input.txt"));

var grid = new List<List<char>>();

var start = new Point();
var end = new Point(); 
{
    int y = 0;
    while (!r.EOF) {
        var line = r.ReadLine();
        var row = new List<char>();
        for (int x = 0; x < line.Length; x++) {
            var ch = line[x];
            if (ch == 'S') {
                start = new Point(x, y);
                ch = 'a';
            } else if (ch == 'E') {
                end = new Point(x, y);
                ch = 'z';
            }
            row.Add(ch);
        }
        grid.Add(row);
        y++;
    }
}

int shortest = astar(grid, start, end);
Console.WriteLine("part 1: " + shortest); // 534


// part 2
int minShortest = int.MaxValue;
for (int y = 0; y < grid.Count; y++) {
    for (int x = 0; x < grid[y].Count; x++) {
        if (grid[y][x] == 'a') {
            Console.WriteLine("try " + new Point(x, y));
            var pathLength = astar(grid, new Point(x, y), end, minShortest);
            minShortest = Math.Min(pathLength, minShortest);
        }
    }
}
Console.WriteLine("part 2: " + minShortest); // 525 on {X=0,Y=29}



int astar(List<List<char>> grid, Point start, Point end, int abortOnPathLength = int.MaxValue) {
    var openlist = new List<Node>();
    var closedlist = new List<Node>();

    var startNode = new Node(Position: start) { Ch = grid[start.Y][start.X] };
    openlist.Add(startNode);

    int i = 0;

    while (openlist.Count > 0) {
        // PrintOpenList(grid, openlist, closedlist);
        var currentNode = GetMinimalNode(openlist.ToList());

        if (CountSteps(currentNode) >= abortOnPathLength) return int.MaxValue;
        
        //if (i % 1000 == 0) PrintPath(grid, currentNode, start, end);
        i++;

        openlist.Remove(currentNode);
        closedlist.Add(currentNode);

        if (currentNode.Position == end) {
            // PrintOpenList(grid, openlist.ToList(), closedlist.ToList());
            PrintPath(grid, currentNode, start, end);
            var x = CountSteps(currentNode);
            Console.WriteLine(x);
            return x;
        }

        var children = GetNeighbors(grid, currentNode.Position).ToList();
        foreach (var childPos in children) {
            if (closedlist.Any(x => x.Position == childPos)) {
                continue;
            }

            var childNode = new Node(Position: childPos) {
                Ch = grid[childPos.Y][childPos.X],
                Parent = currentNode,
                G = currentNode.G + 1,
                H = (Math.Abs(currentNode.Position.X - end.X) + Math.Abs(currentNode.Position.Y - end.Y))
            };

            if (openlist.Any(x => x.Position == childPos && x.G <= childNode.G)) {
                continue;
            }

            openlist.Add(childNode);

        }
    }
    return int.MaxValue;
}

Node GetMinimalNode(List<Node> openlist) {
    var min = int.MaxValue;
    Node n = null;
    foreach (var node in openlist) {
        if (node.F < min) {
            min = node.F;
            n = node;
        }
    }
    return n;
}

int CountSteps(Node currentNode) {
    if (currentNode.Parent == null) return 0;
    return 1 + CountSteps(currentNode.Parent);
}

void PrintPath(List<List<char>> grid, Node currentNode, Point start, Point end) {
    for (int y = 0; y < grid.Count; y++) {
        for (int x = 0; x < grid[y].Count; x++) {
            Node node = FindNode(currentNode, new Point(x, y));
            if (node != null) {
                if (node.Position == start) {
                    Console.Write('S');
                } else if (node.Position == end) {
                    Console.Write('E');
                } else if (node.Parent == null) {
                    Console.Write('+');
                } else if (node.Parent.Position.X > node.Position.X) {
                    Console.Write('<');
                } else if (node.Parent.Position.X < node.Position.X) {
                    Console.Write('>');
                } else if (node.Parent.Position.Y > node.Position.Y) {
                    Console.Write('^');
                } else if (node.Parent.Position.Y < node.Position.Y) {
                    Console.Write('v');
                }
            } else {
                Console.Write(" "); //  grid[y][x]);
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

Node FindNode(Node currentNode, Point position) {
    var n = currentNode;
    while (n != null) {
        if (n.Parent != null && n.Parent.Position == n.Position) throw new Exception("ugl");
        if (n.Position == position) return n;
        n = n.Parent;
    }
    return null;
}

void PrintOpenList(List<List<char>> grid, List<Node> openlist, List<Node> closedlist) {
    for (int y = 0; y < grid.Count; y++) {
        for (int x = 0; x < grid[y].Count; x++) {
            var opennode = openlist.FirstOrDefault(node => node.Position == new Point(x, y));
            var closednode = closedlist.FirstOrDefault(node => node.Position == new Point(x, y));
            if (opennode != null) {
                Console.Write('#');
            } else if (closednode != null) {
                Console.Write('X');
            } else {
                Console.Write('.');
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

IEnumerable<Point> GetNeighbors(List<List<char>> grid, Point pos) {
    int maxY = grid.Count - 1;
    int maxX = grid[0].Count - 1;
    if (pos.X < maxX && grid[pos.Y][pos.X] >= grid[pos.Y][pos.X + 1] - 1) yield return new Point(pos.X + 1, pos.Y);
    if (pos.X > 0 && grid[pos.Y][pos.X] >= grid[pos.Y][pos.X - 1] - 1) yield return new Point(pos.X - 1, pos.Y);
    if (pos.Y < maxY && grid[pos.Y][pos.X] >= grid[pos.Y + 1][pos.X] - 1) yield return new Point(pos.X, pos.Y + 1);
    if (pos.Y > 0 && grid[pos.Y][pos.X] >= grid[pos.Y - 1][pos.X] - 1) yield return new Point(pos.X, pos.Y - 1);
}

class Node {
    public Node(Point Position) {
        this.Position = Position;
    }
    public Node Parent { get; set; }

    public char Ch { get; set; }

    public int G { get; set; } // distance to start node
    public int H { get; set; } // heuristic (optimistic)
    public int F { get => G + H; } // total cost
    public Point Position { get; }

    public override bool Equals(object? obj) {
        return
            ((Node)obj).Position == this.Position
            && ((Node)obj).Parent?.Position == this.Parent?.Position
            // && ((Node)obj).G == this.G
            ;
    }
    public override int GetHashCode() {
        return Position.GetHashCode();
    }
    public override string ToString() {
        return $"{Ch} {Position} G:{G} H:{H} F:{F}";
    }
}