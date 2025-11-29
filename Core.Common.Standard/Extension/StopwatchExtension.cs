using System.Diagnostics;

namespace KY.Core.Extension;

public static class StopwatchExtension
{
    public static string FormattedElapsed(this Stopwatch stopwatch)
    {
        if (stopwatch.ElapsedMilliseconds < 1)
        {
            return "<1 ms";
        }
        return $"{stopwatch.ElapsedMilliseconds} ms";
    }
}
