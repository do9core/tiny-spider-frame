using System.Threading.Tasks;

namespace SpiderFrame.Helpers
{
    public delegate void WaitEventHandler(WaitEventArgs e);

    /// <summary>
    /// 爬虫暂停器接口
    /// </summary>
    public interface IWaitTimer
    {
        /// <summary>
        /// 开始暂停前
        /// </summary>
        event WaitEventHandler OnWait;

        /// <summary>
        /// 暂停结束后
        /// </summary>
        event WaitEventHandler Waited;

        /// <summary>
        /// 指示暂停，true时暂停
        /// </summary>
        bool Peek { get; }

        /// <summary>
        /// 暂停随机事件
        /// </summary>
        /// <returns></returns>
        Task Wait();
    }
}
