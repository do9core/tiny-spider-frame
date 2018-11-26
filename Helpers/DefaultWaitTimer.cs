using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiderFrame.Helpers
{
    /// <summary>
    /// 默认的爬虫暂停timer
    /// </summary>
    public class DefaultWaitTimer : IWaitTimer
    {
        private static readonly Random random = new Random();

        public event WaitEventHandler OnWait;
        public event WaitEventHandler Waited;

        public double PeekLimit { get; set; }

        public int BaseTimeLimitDown { get; set; }
        public int BaseTimeLimitUp { get; set; }

        public FactorOperation FactorOperation { get; set; }

        public List<(int, int)> FactorLimits { get; private set; }

        /// <summary>
        /// 默认25%暂停率
        /// </summary>
        public bool Peek => random.NextDouble() <= PeekLimit;

        /// <summary>
        /// 使用默认参数初始化timer
        /// </summary>
        public DefaultWaitTimer()
        {
            PeekLimit = 0.25;

            BaseTimeLimitDown = 500;
            BaseTimeLimitUp = 700;

            FactorOperation = FactorOperation.Product;

            FactorLimits = new List<(int, int)>
            {
                (10, 25),
                (100, 150)
            };
        }

        /// <summary>
        /// 暂停随机时间，具体时间由参数确定
        /// </summary>
        /// <returns></returns>
        public async Task Wait()
        {
            int time = 1;

            foreach (var limit in FactorLimits)
            {
                (int limitDown, int limitUp) = limit;
                int appendTime = random.Next(limitDown, limitUp);

                switch (FactorOperation)
                {
                    case FactorOperation.Sum: time += appendTime; break;
                    case FactorOperation.Product: time *= appendTime; break;
                }
            }

            time += random.Next(BaseTimeLimitDown, BaseTimeLimitUp);

            WaitEventArgs e = new WaitEventArgs(time);

            OnWait.Invoke(e.Begin());

            await Task.Delay(time);

            Waited.Invoke(e.End());
        }
    }
}
