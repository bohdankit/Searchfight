using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Searchfight.Handlers;
using Searchfight.Results;
using Searchfight.SearchEngine;
using Searchfight.Services;
using Searchfight.TestHelpers;
using Xunit;

namespace Searchfight.Tests.Services
{
    public class FightOrganizerTests
    {
        private readonly FightsOrganizer _fightsOrganizer;

        private readonly Mock<IEngine> _engineMockA;
        private readonly Mock<IEngine> _engineMockB;
        private readonly Mock<IConsoleHandler> _consoleHandlerMock;

        public FightOrganizerTests()
        {
            _engineMockA = new Mock<IEngine>(MockBehavior.Strict);
            _engineMockB = new Mock<IEngine>(MockBehavior.Strict);
            var engines = new List<IEngine>
            {
                _engineMockA.Object,
                _engineMockB.Object
            };

            _engineMockA.Setup(x => x.GetName())
                .Returns(SearchEngineName.Google);
            _engineMockB.Setup(x => x.GetName())
                .Returns(SearchEngineName.Bing);

            _consoleHandlerMock = new Mock<IConsoleHandler>(MockBehavior.Loose);

            _fightsOrganizer = new FightsOrganizer(
                engines,
                _consoleHandlerMock.Object);
        }

        [Theory]
        [AutoMoqData]
        public async Task Fight_ReturnsCorrectResponse(
            string one,
            string two)
        {
            var queries = new[]{one, two};

            var a1 = new SearchResult(SearchEngineName.Google, 1);
            var b1 = new SearchResult(SearchEngineName.Bing, 2);
            var a2 = new SearchResult(SearchEngineName.Google, 4);
            var b2 = new SearchResult(SearchEngineName.Bing, 3);

            _engineMockA.Setup(x => x.GetResultsCountAsync(one))
                .ReturnsAsync(a1);
            _engineMockB.Setup(x => x.GetResultsCountAsync(one))
                .ReturnsAsync(b1);
            _engineMockA.Setup(x => x.GetResultsCountAsync(two))
                .ReturnsAsync(a2);
            _engineMockB.Setup(x => x.GetResultsCountAsync(two))
                .ReturnsAsync(b2);
            
            var searchResults1 = new List<SearchResult>(){a1, b1};
            var result1 = new RequestResult(searchResults1, one);

            var searchResults2 = new List<SearchResult>() { a2, b2 };
            var result2 = new RequestResult(searchResults2, two);

            await _fightsOrganizer.Fight(queries);
            
            _consoleHandlerMock.Verify(m => 
                m.WriteLine($"{SearchEngineName.Google.ToString()} winner: {two}"), Times.Once);
            _consoleHandlerMock.Verify(m =>
                m.WriteLine($"{SearchEngineName.Bing.ToString()} winner: {one}"), Times.Once);
            _consoleHandlerMock.Verify(m =>
                m.WriteLine($"Total winner: {two}"), Times.Once);
            _consoleHandlerMock.Verify(m => m.WriteLine(result1.ToString()), Times.Once);
            _consoleHandlerMock.Verify(m => m.WriteLine(result2.ToString()), Times.Once);
        }

        [Theory]
        [AutoMoqData]
        public async Task Fight_EngineThrowsWebException_ShouldHandleIt(
            string one,
            string two,
            string errorMessage)
        {
            var queries = new[] { one, two };
            
            _engineMockA.Setup(x => x.GetResultsCountAsync(It.IsAny<string>()))
                .ThrowsAsync(new WebException(errorMessage));
            _engineMockB.Setup(x => x.GetResultsCountAsync(It.IsAny<string>()))
                .ThrowsAsync(new WebException(errorMessage));
            
            Task Act() => _fightsOrganizer.Fight(queries);

            var ex = await Assert.ThrowsAsync<WebException>(Act);
            ex.Message.Should().BeEquivalentTo(errorMessage);

            _consoleHandlerMock.Verify(m => m.WriteLine($"Sorry, but trial version of Search API doesn't allow too many requests. Error message: {errorMessage}"));
        }
    }
}
