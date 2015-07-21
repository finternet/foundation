using System;
using System.Diagnostics;

namespace FI.Foundation.Threading
{
    public static class TimerHelpers
    {
        public static long Elapsed(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action.Invoke();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}
