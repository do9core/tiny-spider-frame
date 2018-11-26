using System;
using System.Collections.Generic;

namespace SpiderFrame.Downloader
{
    public class DownloadCompleteEventArgs
    {
        public DownloadResult Result { get; set; }
        public object DownloadSource { get; set; }
        public Type SourceType { get; set; }
        public IList<Exception> Exceptions { get; set; }
    }
}
