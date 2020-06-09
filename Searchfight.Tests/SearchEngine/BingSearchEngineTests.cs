using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Searchfight.Handlers;
using Searchfight.Results;
using Searchfight.SearchEngine;
using Searchfight.TestHelpers;
using Xunit;

namespace Searchfight.Tests.SearchEngine
{
    public class BingSearchEngineTests
    {
        private readonly BingSearchEngine _bingSearchEngine;
        private readonly Mock<IWebRequestHandler> _webRequestHandlerMock;

        public BingSearchEngineTests()
        {
            ConfigurationManager.AppSettings["BingCustomConfig"] = "TestBingCustomConfig";
            ConfigurationManager.AppSettings["BingAccessKey"] = "TestBingAccessKey";
            _webRequestHandlerMock = new Mock<IWebRequestHandler>(MockBehavior.Strict);
            _bingSearchEngine = new BingSearchEngine(_webRequestHandlerMock.Object);
        }

        [Fact]
        public void GetName_ReturnsGoogle()
        {
            var expected = SearchEngineName.Bing;

            var actual = _bingSearchEngine.GetName();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetCountFromJson_ReturnsTotalResults(BigInteger totalResults, string searchQuery, string header)
        {
            var jsonString = "{webPages:{totalEstimatedMatches: " + totalResults + "}}";
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            var responseStream = new MemoryStream();
            responseStream.Write(jsonBytes, 0, jsonBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var response = new Mock<HttpWebResponse>();
            response.Setup(c => c.GetResponseStream()).Returns(responseStream);

            var request = new Mock<HttpWebRequest>();
            request.Setup(c => c.GetResponse()).Returns(response.Object);
            request.SetupGet(x => x.Headers).Returns(
                new WebHeaderCollection {
                    {"Ocp-Apim-Subscription-Key", header}
                });
           
            var url = $"https://api.cognitive.microsoft.com/bingcustomsearch/v7.0/search?q={Uri.EscapeDataString(searchQuery)}&customconfig={ConfigurationManager.AppSettings["BingCustomConfig"]}";

            _webRequestHandlerMock.Setup(c => c.Create(url))
                .Returns(request.Object);

            _webRequestHandlerMock.Setup(c => c.GetResponseAsync(request.Object))
                .ReturnsAsync(response.Object);

            var expected = new SearchResult(SearchEngineName.Bing, totalResults);

            var actual = await _bingSearchEngine.GetResultsCountAsync(searchQuery);

            actual.Should().BeEquivalentTo(expected);
            request.Object.Headers["Ocp-Apim-Subscription-Key"].Should()
                .BeEquivalentTo(ConfigurationManager.AppSettings["BingAccessKey"]);
        }
    }
}
