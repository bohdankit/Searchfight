using System.Collections.Generic;
using Searchfight.SearchEngine;

namespace Searchfight.Results
{
    public struct CompetitionResult
    {
        public SearchEngineName EngineName;
        public List<string> Queries;

        public CompetitionResult(SearchEngineName name, List<string> queries)
        {
            EngineName = name;
            Queries = queries;
        }
    }
}