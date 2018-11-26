using System;

namespace SpiderFrame.Helpers
{
    public class WaitEventArgs
    {
        public int TotalTime { get; }
        public DateTime BeginTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public WaitEventArgs(int totalTime)
        {
            TotalTime = totalTime;
        }

        public WaitEventArgs Begin()
        {
            if (BeginTime == null)
            {
                BeginTime = DateTime.Now;
            }

            return this;
        }

        public WaitEventArgs End()
        {
            if (EndTime == null)
            {
                EndTime = DateTime.Now;
            }

            return this;
        }
    }
}
