using utils;

var r = new MyReader(File.OpenText("small.txt"));

var scorePart1 = 0;
var scorePart2 = 0;
while (!r.EOF)
{
    var first = r.ReadWord();
    var second = r.ReadWord();
    scorePart1 += PlayerHandScore(hand: second) + Score(player: second, opponent: first);
    scorePart2 += WinnerScore(hand: second) + PlayerHandScore(WhatToPlay(strategy: second, opponent: first));
}
Console.WriteLine("part1: " + scorePart1);
Console.WriteLine("part2: " + scorePart2);

int PlayerHandScore(string hand) =>
    hand switch {
        "X" => 1,
        "Y" => 2,
        "Z" => 3,
    };

int WinnerScore(string hand) =>
    hand switch {
        "X" => 0,
        "Y" => 3,
        "Z" => 6
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

string WhatToPlay(string strategy, string opponent) {
    if (strategy == "X") { // lose
        if (opponent == "A") return "Z";
        if (opponent == "B") return "X";
        if (opponent == "C") return "Y";
    }
    if (strategy == "Y") { // draw
        if (opponent == "A") return "X";
        if (opponent == "B") return "Y";
        if (opponent == "C") return "Z";
    }
    if (strategy == "Z") { // win
        if (opponent == "A") return "Y";
        if (opponent == "B") return "Z";
        if (opponent == "C") return "X";
    }
    return "err";
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

/*
Part Two:
X: you lose
Y: you draw
Z: you win
*/