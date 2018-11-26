using SpiderFrame.Downloader;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpiderFrame
{
    public abstract class SpiderBase
    {
        protected HttpClient _client;
        protected IDownloader _downloader;
        protected ISourceParser _sourceParser;
        protected IFolderNameParser _folderNameParser;

        public event OnDownloadEventHandler OnDownload
        {
            add
            {
                _downloader.OnDownload += value;
            }

            remove
            {
                _downloader.OnDownload -= value;
            }
        }
        public event DownloadCompletedEventHandler DownloadComplete
        {
            add
            {
                _downloader.DownloadComplete += value;
            }

            remove
            {
                _downloader.DownloadComplete -= value;
            }
        }

        public SpiderBase(
            HttpClient client,
            IDownloader downloader,
            ISourceParser sourceParser,
            IFolderNameParser folderNameParser)
        {
            _client = client;
            _sourceParser = sourceParser;
            _downloader = downloader;
            _folderNameParser = folderNameParser;
        }

        /// <summary>
        /// 使用downloader下载的模板方法
        /// </summary>
        /// <param name="entrance">入口url</param>
        /// <returns></returns>
        public async Task DownloadFrom(string entrance)
        {
            var sourceArgument = GetSourceParserArgument(entrance);
            var sources = await _sourceParser.GetSourcesAsync(sourceArgument);

            var folderArgument = GetFolderNameParserArgument(entrance);
            var folderName = await _folderNameParser.GetFolderNameAsync(folderArgument);

            await _downloader.DownloadAsync(sources, new System.IO.DirectoryInfo(folderName));
        }

        protected virtual object GetSourceParserArgument(object input) => input;
        protected virtual object GetFolderNameParserArgument(object input) => input;
        public virtual void CancelDownload() => _downloader.Cancel();
    }
}
