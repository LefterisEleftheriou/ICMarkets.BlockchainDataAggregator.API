using ICMarkets.BlockchainDataAggregator.Application.Interfaces;
using ICMarkets.BlockchainDataAggregator.Application.Services;
using ICMarkets.BlockchainDataAggregator.Domain.Interfaces;
using ICMarkets.BlockchainDataAggregator.Domain;
using Moq;
using Microsoft.Extensions.Logging;

namespace ICMarkets.BlockchainDataAggregator.Application.UnitTests.Services
{
    [TestFixture]
    public class BlockchainServiceTests
    {
        private Mock<IBlockchainRepository> _mockRepository;
        private Mock<HttpClient> _mockHttpClient;
        private Mock<ILogger<BlockchainService>> _mockLogger;
        private IBlockchainService _blockchainService;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IBlockchainRepository>();
            _mockHttpClient = new Mock<HttpClient>();
            _mockLogger = new Mock<ILogger<BlockchainService>>();
            _blockchainService = new BlockchainService(_mockRepository.Object, _mockHttpClient.Object, _mockLogger.Object);
        }

        // ...

        [Test]
        public async Task GetLatestBlockchainDataAsync_ShouldReturnStoredData_WhenCurrencyIsValid()
        {
            // Arrange
            var sampleData = new List<BlockchainData>
            {
                new BlockchainData("btc.main", 123456, "testHash", "2025-02-02T08:50:05Z", "latestUrl",
                    "prevHash", "prevUrl", 10, 5, 1000, 500, 200, 900, "forkHash")
            };

            _mockRepository.Setup(repo => repo.GetLatestAsync("btc.main", 10))
                .ReturnsAsync(sampleData);

            // Act
            var result = await _blockchainService.GetLatestBlockchainDataAsync("btc.main", 10);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("btc.main"));
        }
    }
}
