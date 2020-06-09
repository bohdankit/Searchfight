using System;
using System.Configuration;
using System.IO;
using System.Net;
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
    public class GoogleSearchEngineTests
    {
        private readonly GoogleSearchEngine _googleSearchEngine;
        private readonly Mock<IWebRequestHandler> _webRequestHandlerMock;

        public GoogleSearchEngineTests()
        {
            ConfigurationManager.AppSettings["GoogleCx"] = "TestGoogleCx";
            ConfigurationManager.AppSettings["GoogleApiKey"] = "TestGoogleApiKey";
            _webRequestHandlerMock = new Mock<IWebRequestHandler>(MockBehavior.Strict);
            _googleSearchEngine = new GoogleSearchEngine(_webRequestHandlerMock.Object);
        }

        [Fact]
        public void GetName_ReturnsGoogle()
        {
            var expected = SearchEngineName.Google;

            var actual = _googleSearchEngine.GetName();

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetCountFromJson_ReturnsTotalResults(BigInteger totalResults, string searchQuery)
        {
            var jsonString = "{searchInformation:{totalResults: " + totalResults + "}}";
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            var responseStream = new MemoryStream();
            responseStream.Write(jsonBytes, 0, jsonBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var response = new Mock<HttpWebResponse>();
            response.Setup(c => c.GetResponseStream()).Returns(responseStream);

            var request = new Mock<HttpWebRequest>();
            request.Setup(c => c.GetResponse()).Returns(response.Object);

            var url =
                $"https://www.googleapis.com/customsearch/v1?key={ConfigurationManager.AppSettings["GoogleApiKey"]}&cx={ConfigurationManager.AppSettings["GoogleCx"]}&q={searchQuery}";

            _webRequestHandlerMock.Setup(c => c.Create(url))
                .Returns(request.Object);

            _webRequestHandlerMock.Setup(c => c.GetResponseAsync(request.Object))
                .ReturnsAsync(response.Object);

            var expected = new SearchResult(SearchEngineName.Google, totalResults);

            var actual = await _googleSearchEngine.GetResultsCountAsync(searchQuery);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [AutoMoqData]
        public async Task GetCountFromJson_ResponseStreamAbsent_ThrowsInvalidOperationException(string searchQuery)
        {
            var response = new Mock<HttpWebResponse>();
            response.Setup(c => c.GetResponseStream()).Returns((Stream)null);

            var request = new Mock<HttpWebRequest>();
            request.Setup(c => c.GetResponse()).Returns(response.Object);

            var url =
                $"https://www.googleapis.com/customsearch/v1?key={ConfigurationManager.AppSettings["GoogleApiKey"]}&cx={ConfigurationManager.AppSettings["GoogleCx"]}&q={searchQuery}";

            _webRequestHandlerMock.Setup(c => c.Create(url))
                .Returns(request.Object);

            _webRequestHandlerMock.Setup(c => c.GetResponseAsync(request.Object))
                .ReturnsAsync(response.Object);

            Task Act() => _googleSearchEngine.GetResultsCountAsync(searchQuery);

            Exception ex = await Assert.ThrowsAsync<InvalidOperationException>(Act);
            ex.Message.Should().BeEquivalentTo("Cannot read from stream");
        }
    }
}
