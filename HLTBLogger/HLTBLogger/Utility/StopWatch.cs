using System;
using System.Collections.Generic;
using System.Text;

namespace HLTBLogger.Utility
{
    public class StopWatch
    {
        public bool IsTicking { get => startTime != default; }
        public TimeSpan Elapsed
        {
            get
            {
                if (startTime == default) return cumulativeElapsed;
                return cumulativeElapsed + (DateTime.UtcNow - startTime);
            }
        }

        private TimeSpan cumulativeElapsed = new TimeSpan();
        private DateTime startTime;
        public void Start()
        {
            startTime = DateTime.UtcNow;
        }

        public void Stop()
        {
            cumulativeElapsed += Elapsed;
            startTime = default;
        }

        public void Reset()
        {
            cumulativeElapsed = new TimeSpan();
            startTime = default;
        }
    }
}
