using System.Net;
using System.Threading.Tasks;

namespace Searchfight.Handlers
{
    public interface IWebRequestHandler
    {
        WebRequest Create(string url);
        Task<WebResponse> GetResponseAsync(WebRequest webRequest);
    }
}