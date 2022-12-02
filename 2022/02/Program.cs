using utils;

var r = new MyReader(File.OpenText("small.txt"));

var score = 0;
while (!r.EOF)
{
    var opponent = r.ReadWord();
    var player = r.ReadWord();
    score += PlayerHandScore(player) + Score(player, opponent);
}
Console.WriteLine(score);

int PlayerHandScore(string hand) =>
    hand switch {
        "X" => 1,
        "Y" => 2,
        "Z" => 3,
    };

int Score (string player, string opponent) {
    if (player == "X") {
        if (opponent == "A") return 3; // draw
        if (opponent == "B") return 0; // loss
        if (opponent == "C") return 6; // win
    }
    if (player == "Y") {
        if (opponent == "A") return 6;
        if (opponent == "B") return 3;
        if (opponent == "C") return 0;
    }
    if (player == "Z") {
        if (opponent == "A") return 0;
        if (opponent == "B") return 6;
        if (opponent == "C") return 3;
    }
    return -1;
}

/*
player
X: Rock
Y: Paper
Z: Scissors

opponent
A: Rock
B: Paper
C: Scissors
*/