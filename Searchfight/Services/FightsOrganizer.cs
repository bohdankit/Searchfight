using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Searchfight.Handlers;
using Searchfight.Results;
using Searchfight.SearchEngine;

namespace Searchfight.Services
{
    public class FightsOrganizer
    {
        private readonly List<IEngine> _engines;
        private readonly List<CompetitionResult> _competitionResults;
        private readonly IConsoleHandler _consoleHandler;

        private string _totalWinner = "";
        public FightsOrganizer(
            List<IEngine> engines, 
            IConsoleHandler consoleHandler)
        {
            _engines = engines;
            _consoleHandler = consoleHandler;
            _competitionResults = new List<CompetitionResult>();
            foreach (var engine in _engines)
            {
                _competitionResults.Add(new CompetitionResult(engine.GetName(), new List<string>()));
            }
        }

        public async Task Fight(string[] queries)
        {
            BigInteger totalWinnerCount = 0;

            var searchTasks = queries.Select(GetSearchResults).ToList();

            while (searchTasks.Count > 0)
            {
                var finishedSearch = await Task.WhenAny(searchTasks);
                
                var fightResult = await finishedSearch;
                _consoleHandler.WriteLine(fightResult.ToString());

                var fightWinnerResult = fightResult.SearchResults
                    .OrderByDescending(item => item.Count).First();
                if (fightWinnerResult.Count > totalWinnerCount)
                {
                    _totalWinner = fightResult.RequestQuery;
                }
                var winnerEngine = _competitionResults.First(x => x.EngineName == fightWinnerResult.EngineName);
                winnerEngine.Queries.Add(fightResult.RequestQuery);

                searchTasks.Remove(finishedSearch);
            }

            WriteResults();
        }

        private async Task<RequestResult> GetSearchResults(string query)
        {
            var searchTasks = new List<Task<SearchResult>>();
            var searchResults = new List<SearchResult>();
            foreach (var engine in _engines)
            {
                searchTasks.Add(engine.GetResultsCountAsync(query));
            }

            while (searchTasks.Count > 0)
            {
                var finishedSearch = await Task.WhenAny(searchTasks);
                try
                {
                    var finishedSearchResult = await finishedSearch;
                    searchResults.Add(finishedSearchResult);
                    searchTasks.Remove(finishedSearch);
                }
                catch (WebException e)
                {
                    _consoleHandler.WriteLine($"Sorry, but trial version of Search API doesn't allow too many requests. Error message: {e.Message}");
                    throw;
                }
            }

            return new RequestResult(searchResults, query);
        }

        private void WriteResults()
        {
            foreach (var result in _competitionResults)
            {
                _consoleHandler.WriteLine($"{result.EngineName.ToString()} winner: "
                                  + string.Join(' ', result.Queries));
            }
            _consoleHandler.WriteLine($"Total winner: {_totalWinner}");
        }
    }
}
