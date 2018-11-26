using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using SpiderFrame.Helpers;

namespace SpiderFrame.Downloader
{
    public class DefaultDownloader : IDownloader
    {
        protected const int BufferSize = 20 * 1024;

        protected readonly HttpClient _client;
        protected readonly IFileNameParser _fileNameParser;
        protected readonly IWaitTimer _waitTimer;

        public event OnDownloadEventHandler OnDownload;
        public event DownloadCompletedEventHandler DownloadComplete;

        protected CancellationTokenSource _cancel;

        /// <summary>
        /// 使用默认wait timer初始化downloader
        /// </summary>
        /// <param name="client">供downloader使用的HttpClient</param>
        /// <param name="fileNameParser">文件名parser</param>
        public DefaultDownloader(HttpClient client, IFileNameParser fileNameParser)
         : this(client, fileNameParser, new DefaultWaitTimer()) { }

        /// <summary>
        /// 使用给定timer构造downloader
        /// </summary>
        /// <param name="client">供downloader使用的HttpClient</param>
        /// <param name="fileNameParser">文件名parser</param>
        /// <param name="waitTimer">爬虫暂停timer</param>
        public DefaultDownloader(HttpClient client, IFileNameParser fileNameParser, IWaitTimer waitTimer)
        {
            _client = client;
            _fileNameParser = fileNameParser;
            _waitTimer = waitTimer;
            _cancel = new CancellationTokenSource();
        }

        public async Task DownloadAsync(IEnumerable<string> sources, DirectoryInfo directory)
        {
            _cancel = new CancellationTokenSource();

            if (!directory.Exists) directory.Create();

            OnDownload?.Invoke(new DownloadBeginEventArgs
            {
                DownloadSource = sources,
                SourceType = typeof(IEnumerable<string>)
            });

            var result = new DownloadCompleteEventArgs
            {
                Exceptions = new List<Exception>(),
                SourceType = typeof(IEnumerable<string>),
                DownloadSource = sources
            };

            foreach (var url in sources)
            {
                if (_cancel.IsCancellationRequested)
                {
                    result.Result = DownloadResult.Canceled;
                    DownloadComplete.Invoke(result);
                    return;
                }

                try
                {
                    await DownloadAsync(url, directory);

                    if (_waitTimer.Peek)
                    {
                        await _waitTimer.Wait();
                    }
                }
                catch (Exception e)
                {
                    result.Exceptions.Add(e);
                }
            }

            if (result.Exceptions.Count > 0)
            {
                result.Result = DownloadResult.Exception;
                DownloadComplete?.Invoke(result);
                return;
            }

            result.Result = DownloadResult.Success;
            DownloadComplete?.Invoke(result);
        }

        public async Task DownloadAsync(string source, DirectoryInfo directory)
        {
            string fileName = _fileNameParser.GetFileName(source);
            string fullName = $"{directory.FullName}/{fileName}";

            using (Stream webStream = await _client.GetStreamAsync(source))
            {
                using (Stream fileStream = File.Create(fullName))
                {
                    await webStream.CopyToAsync(fileStream, BufferSize, _cancel.Token);
                }
            }
        }

        public void Cancel()
        {
            _cancel.Cancel();
        }
    }
}
