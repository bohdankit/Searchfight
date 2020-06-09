using System;
using System.IO;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Searchfight.Handlers;
using Searchfight.Results;

namespace Searchfight.SearchEngine
{
    public abstract class Engine : IEngine
    {
        protected IWebRequestHandler WebRequestHandler;

        protected Engine(IWebRequestHandler webRequestHandler)
        {
            WebRequestHandler = webRequestHandler;
        }

        public async Task<SearchResult> GetResultsCountAsync(string searchQuery)
        {
            var webRequest = GetWebRequest(searchQuery);
            var json = await GetResponse(webRequest);
            var count = GetCountFromJson(json);
            return new SearchResult(GetName(), count);
        }
        public abstract SearchEngineName GetName();

        private async Task<dynamic> GetResponse(WebRequest webRequest)
        {
            var response = (HttpWebResponse)await WebRequestHandler.GetResponseAsync(webRequest);
            var responseString = new StreamReader(response.GetResponseStream()
                                                  ?? throw new InvalidOperationException("Cannot read from stream")).ReadToEnd();
            return JsonConvert.DeserializeObject(responseString);
        }

        protected abstract BigInteger GetCountFromJson(dynamic json);

        protected abstract WebRequest GetWebRequest(string searchQuery);

    }
}