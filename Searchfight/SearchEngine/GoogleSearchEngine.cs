using System;
using System.Configuration;
using System.Net;
using System.Numerics;
using Searchfight.Handlers;

namespace Searchfight.SearchEngine
{
    public class GoogleSearchEngine : Engine
    {
        private const string UriBase = "https://www.googleapis.com/customsearch/v1?key={0}&cx={1}&q={2}";
        private readonly string _cx;
        private readonly string _apiKey;

        public GoogleSearchEngine(IWebRequestHandler webRequestHandler) : base(webRequestHandler)
        {
            _cx = ConfigurationManager.AppSettings["GoogleCx"];
            _apiKey = ConfigurationManager.AppSettings["GoogleApiKey"];
        }

        public override SearchEngineName GetName()
        {
            return SearchEngineName.Google;
        }

        protected override BigInteger GetCountFromJson(dynamic json)
        {
            return json.searchInformation.totalResults;
        }

        protected override WebRequest GetWebRequest(string searchQuery)
        {
            return WebRequestHandler.Create(string.Format(UriBase, _apiKey, _cx, Uri.EscapeDataString(searchQuery)));
        }
    }
}
