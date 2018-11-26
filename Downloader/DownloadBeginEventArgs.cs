using System;

namespace SpiderFrame.Downloader
{
    public class DownloadBeginEventArgs
    {
        public object DownloadSource { get; set; }
        public Type SourceType { get; set; }
    }
}
