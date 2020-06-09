using System.Numerics;
using Searchfight.SearchEngine;

namespace Searchfight.Results
{
    public struct SearchResult
    {
        public SearchEngineName EngineName;
        public BigInteger Count;

        public SearchResult(SearchEngineName name, BigInteger count)
        {
            EngineName = name;
            Count = count;
        }

        public override string ToString()
        {
            return $"{EngineName}: {Count}";
        }
    }
}