using System;
using System.Diagnostics;

namespace DotNetIdeas.SupportClasses
{
    public class DisposableStopwatch : IDisposable
    {
        private readonly Action<long> _elapsedMillisecondsAction;
        private readonly Stopwatch _stopwatch;

        public DisposableStopwatch(Action<long> elapsedMillisecondsAction)
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _elapsedMillisecondsAction = elapsedMillisecondsAction;
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _elapsedMillisecondsAction(_stopwatch.ElapsedMilliseconds);
        }
    }
}