using System.Threading.Tasks;

namespace SpiderFrame
{
    public interface IFolderNameParser
    {
        string GetFolderName(object param);
        Task<string> GetFolderNameAsync(object param);
    }
}
