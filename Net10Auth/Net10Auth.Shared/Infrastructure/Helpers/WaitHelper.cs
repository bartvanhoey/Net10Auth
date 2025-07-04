using System.Diagnostics;
using static System.Threading.Thread;

namespace Net10Auth.Shared.Infrastructure.Helpers;

public static class WaitHelper
{
    public static void WaitSomeSeconds(int seconds = 5)
    {
        var stopwatch = Stopwatch.StartNew();
        while (true)
        {
            if (stopwatch.Elapsed.TotalSeconds >= seconds) break;
            Sleep(500); // Wait for 500ms
        }
        stopwatch.Stop();
    }
}