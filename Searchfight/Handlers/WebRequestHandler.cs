using System.Net;
using System.Threading.Tasks;

namespace Searchfight.Handlers
{
    public class WebRequestHandler : IWebRequestHandler
    {
        public WebRequest Create(string url)
        {
            return WebRequest.Create(url);
        }

        public Task<WebResponse> GetResponseAsync(WebRequest webRequest)
        {
            return webRequest.GetResponseAsync();
        }
    }
}
