using utils;

var r = new MyReader(File.OpenText("large.txt"));

var slidingWindow = new Queue<int>();
var lastSum = 0;
var inc = 0;
while (!r.EOF) {
    var num = r.ReadInt();
    slidingWindow.Enqueue(num);
    if (slidingWindow.Count > 3) {
        slidingWindow.Dequeue();
        int curSum = slidingWindow.Sum();
        if (curSum > lastSum) inc++;
        lastSum = curSum;
    }
}

Console.WriteLine(inc);
