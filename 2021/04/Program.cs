using utils;

var r = new MyReader(File.OpenText("large.txt"));
using var stopwatch = AutoStopwatch.Start();

var randNumbers = r.ReadLine().Split(',').Select(x => int.Parse(x)).ToArray();

int boardSize = 5;
var boards = new List<int[,]>();
var marked = new List<bool[,]>();

while (!r.EOF) {
    var board = new int[boardSize, boardSize];
    for (int i = 0; i < boardSize; i++) {
        for (int j = 0; j < boardSize; j++) {
            board[i, j] = r.ReadInt();
        }
    }
    boards.Add(board);
    marked.Add(new bool[boardSize, boardSize]);
}

(int firstWinningBoardNr, int firstWinnerScore) = FindWinner(true);
Console.WriteLine($"First BINGO on board {firstWinningBoardNr}: {firstWinnerScore}");

(int lastWinningBoardNr, int lastWinnerScore) = FindWinner(false);
Console.WriteLine($"Last BINGO on board {lastWinningBoardNr}: {lastWinnerScore}");


(int winningBoardNr, int score) FindWinner(bool firstWinner) {
    int latestWinnerBoard = -1;
    int latestWinnerScore = -1;
    var boardWon = new bool[boards.Count];

    foreach (var number in randNumbers) {
        for (int i = 0; i < boards.Count; i++) {
            if (boardWon[i]) continue; // skip board that already won
            MarkNumber(boards[i], marked[i], number);

            bool bingo = IsBingo(marked[i]);
            if (bingo) {
                var sumOfUnmarked = SumOfUnmarked(boards[i], marked[i]);
                latestWinnerBoard = i;
                latestWinnerScore = sumOfUnmarked * number;
                boardWon[i] = true;
                if (firstWinner) return (latestWinnerBoard, latestWinnerScore);
            }
        }
    }
    return (latestWinnerBoard, latestWinnerScore);
}

int SumOfUnmarked(int[,] board, bool[,] marked) {
    var sum = 0;
    for (int i = 0; i < boardSize; i++) {
        for (int j = 0; j < boardSize; j++) {
            if (!marked[i, j]) sum += board[i, j];
        }
    }
    return sum;
}

bool IsBingo(bool[,] marked) {
    for (int i = 0; i < boardSize; i++) {
        bool rowBingo = true;
        for (int j = 0; j < boardSize; j++) {
            rowBingo &= marked[i, j];
            if (!rowBingo) break;
        }
        if (rowBingo) return true;
    }
    for (int j = 0; j < boardSize; j++) {
        bool colBingo = true;
        for (int i = 0; i < boardSize; i++) {
            colBingo &= marked[i, j];
            if (!colBingo) break;
        }
        if (colBingo) return true;
    }
    return false;
}

void MarkNumber(int[,] board, bool[,] marked, int number) {
    for (int i = 0; i < boardSize; i++) {
        for (int j = 0; j < boardSize; j++) {
            if (board[i, j] == number) marked[i, j] = true;
        }
    }
}
