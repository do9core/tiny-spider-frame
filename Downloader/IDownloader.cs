using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SpiderFrame.Downloader
{
    public delegate void OnDownloadEventHandler(DownloadBeginEventArgs e);
    public delegate void DownloadCompletedEventHandler(DownloadCompleteEventArgs e);

    /// <summary>
    /// 爬虫下载器接口
    /// </summary>
    public interface IDownloader
    {
        /// <summary>
        /// 开始下载前
        /// </summary>
        event OnDownloadEventHandler OnDownload;

        /// <summary>
        /// 下载完成后
        /// </summary>
        event DownloadCompletedEventHandler DownloadComplete;

        /// <summary>
        /// 以异步方式下载所有数据
        /// </summary>
        /// <param name="sources">可遍历的源</param>
        /// <returns></returns>
        Task DownloadAsync(IEnumerable<string> sources, DirectoryInfo directory);

        /// <summary>
        /// 以异步方式下载目标数据
        /// </summary>
        /// <param name="source">数据源</param>
        /// <returns></returns>
        Task DownloadAsync(string source, DirectoryInfo directory);

        /// <summary>
        /// 取消下载
        /// </summary>
        void Cancel();
    }
}
