using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Searchfight.Handlers;
using Searchfight.SearchEngine;
using Searchfight.Services;

namespace Searchfight
{
    class Program
    {
        static async Task Main(string[] queries)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IWebRequestHandler, WebRequestHandler>()
                .AddSingleton<IConsoleHandler, ConsoleHandler>()
                .BuildServiceProvider();

            var webRequestHandler =
                serviceProvider.GetService<IWebRequestHandler>();

            var engines = new List<IEngine>
            {
                new GoogleSearchEngine(webRequestHandler),
                new BingSearchEngine(webRequestHandler)
            };

            var consoleHandler =
                serviceProvider.GetService<IConsoleHandler>();

            var fightsOrganizer = new FightsOrganizer(engines, consoleHandler);
            await fightsOrganizer.Fight(queries);
        }
    }
}
