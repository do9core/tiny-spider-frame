using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpiderFrame
{
    public interface ISourceParser
    {
        IEnumerable<string> GetSources(object input);
        Task<IEnumerable<string>> GetSourcesAsync(object input);
    }
}
