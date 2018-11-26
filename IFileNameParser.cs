using System.Threading.Tasks;

namespace SpiderFrame
{
    public interface IFileNameParser
    {
        string GetFileName(object param);
        Task<string> GetFileNameAsync(object param);
    }
}
