using System.Collections.Generic;
using System.Numerics;
using Searchfight.SearchEngine;

namespace Searchfight.Results
{
    public class CompetitionResult
    {
        public SearchEngineName EngineName;
        public string Query;
        public BigInteger Count;

        public CompetitionResult(SearchEngineName name)
        {
            EngineName = name;
            Query = string.Empty;
            Count = 0;
        }

        public void SetCountAndQuery(BigInteger count, string query)
        {
            Count = count;
            Query = query;
        }

        public override string ToString()
        {
            return $"{EngineName.ToString()} winner: {Query}";
        }
    }
}