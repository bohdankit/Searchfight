using System.Collections.Generic;
using System.Linq;

namespace Searchfight.Results
{
    public struct RequestResult
    {
        public List<SearchResult> SearchResults;
        public string RequestQuery;

        public RequestResult(List<SearchResult> searchResults, string query)
        {
            SearchResults = searchResults;
            RequestQuery = query;
        }

        public override string ToString()
        {
            return RequestQuery + ": " +
                   string.Join(' ', SearchResults.Select(x => x.ToString()));
        }
    }
}