using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace utils;

public class AutoStopwatch : IDisposable {
    private readonly Stopwatch stopwatch;

    public AutoStopwatch() {
        this.stopwatch = Stopwatch.StartNew();
    }

    public void Dispose() {
        Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
    }

    public static AutoStopwatch Start() {
        return new AutoStopwatch();
    }
}

