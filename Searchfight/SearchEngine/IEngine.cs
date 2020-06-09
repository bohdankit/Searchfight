using System.Threading.Tasks;
using Searchfight.Results;

namespace Searchfight.SearchEngine
{
    public interface IEngine
    {
        Task<SearchResult> GetResultsCountAsync(string searchQuery); 
        SearchEngineName GetName();
    }
}