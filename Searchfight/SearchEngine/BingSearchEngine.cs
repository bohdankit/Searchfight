using System;
using System.Configuration;
using System.Net;
using System.Numerics;
using Searchfight.Handlers;

namespace Searchfight.SearchEngine
{
    public class BingSearchEngine : Engine
    {
        private const string UriBase =
            "https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search?q={0}&customconfig={1}";

        private readonly string _customConfig;
        private readonly string _accessKey;

        public BingSearchEngine(IWebRequestHandler webRequestHandler) : base(webRequestHandler)
        {
            _customConfig = ConfigurationManager.AppSettings["BingCustomConfig"];
            _accessKey = ConfigurationManager.AppSettings["BingAccessKey"];
        }

        public override SearchEngineName GetName()
        {
            return SearchEngineName.Bing;
        }

        protected override BigInteger GetCountFromJson(dynamic json)
        {
            return json.webPages.totalEstimatedMatches;
        }

        protected override WebRequest GetWebRequest(string searchQuery)
        {
            var uriQuery = string.Format(UriBase, Uri.EscapeDataString(searchQuery), _customConfig);
            var request = WebRequestHandler.Create(uriQuery);
            request.Headers["Ocp-Apim-Subscription-Key"] = _accessKey;
            return request;
        }
    }
}