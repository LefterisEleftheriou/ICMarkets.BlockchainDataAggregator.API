using System.Net;
using ICMarkets.BlockchainDataAggregator.Application.DTOs;
using ICMarkets.BlockchainDataAggregator.Application.Exceptions;
using ICMarkets.BlockchainDataAggregator.Application.Interfaces;
using ICMarkets.BlockchainDataAggregator.Application.Services;
using ICMarkets.BlockchainDataAggregator.Domain;
using ICMarkets.BlockchainDataAggregator.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace ICMarkets.BlockchainDataAggregator.Application.FunctionalTests.Services
{
    [TestFixture]
    public class BlockchainServiceFunctionalTests
    {
        private Mock<IBlockchainRepository> _mockRepository;
        private Mock<ILogger<BlockchainService>> _mockLogger;
        private Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private Mock<HttpClient> _mockHttpClient;
        private IBlockchainService _blockchainService;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IBlockchainRepository>();
            _mockLogger = new Mock<ILogger<BlockchainService>>();

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _mockHttpClient = new Mock<HttpClient>();

            _blockchainService = new BlockchainService(_mockRepository.Object, _mockHttpClient.Object, _mockLogger.Object);
        }

        /// <summary>
        /// Ensures that when blockchain data does not exist, it is fetched from the API and stored.
        /// </summary>
        [Test]
        public async Task GetLatestBlockchainDataAsync_ShouldFetchFromApi_WhenDataNotExists()
        {
            // Arrange
            var currency = "btc.main";
            var sampleApiResponse = new BlockchainDataDto(
                "btc.main", 123456, "testHash", "2025-02-02T08:50:05Z", "latestUrl",
                "prevHash", "prevUrl", 10, 5, 1000, 500, 200, 900, "forkHash", DateTime.UtcNow);

            var jsonResponse = JsonConvert.SerializeObject(sampleApiResponse);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            _mockRepository.Setup(repo => repo.FindByHashOrHeightAsync(sampleApiResponse.Hash, sampleApiResponse.Height))
                .ReturnsAsync((BlockchainData)null);

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<BlockchainData>()));

            _mockRepository.Setup(repo => repo.GetLatestAsync(currency, 10))
                .ReturnsAsync(new List<BlockchainData> { new BlockchainData(
                    sampleApiResponse.Name, sampleApiResponse.Height, sampleApiResponse.Hash, sampleApiResponse.Time,
                    sampleApiResponse.LatestUrl, sampleApiResponse.PreviousHash, sampleApiResponse.PreviousUrl,
                    sampleApiResponse.PeerCount, sampleApiResponse.UnconfirmedCount, sampleApiResponse.HighFeePerKb,
                    sampleApiResponse.MediumFeePerKb, sampleApiResponse.LowFeePerKb, sampleApiResponse.LastForkHeight,
                    sampleApiResponse.LastForkHash) });

            // Act
            var result = await _blockchainService.GetLatestBlockchainDataAsync(currency, 10);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("btc.main"));

            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<BlockchainData>()), Times.Once);
        }

        /// <summary>
        /// Ensures that if blockchain data already exists in the database, it is returned without calling the API.
        /// </summary>
        [Test]
        public async Task GetLatestBlockchainDataAsync_ShouldReturnStoredData_WhenDataExists()
        {
            // Arrange
            var currency = "eth.main";
            var storedData = new List<BlockchainData>
            {
                new BlockchainData("eth.main", 123456, "testHash", "2025-02-02T08:50:05Z", "latestUrl",
                    "prevHash", "prevUrl", 10, 5, 1000, 500, 200, 900, "forkHash")
            };

            _mockRepository.Setup(repo => repo.GetLatestAsync(currency, 10))
                .ReturnsAsync(storedData);

            // Act
            var result = await _blockchainService.GetLatestBlockchainDataAsync(currency, 10);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("eth.main"));

            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync", Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        /// <summary>
        /// Ensures that an exception is thrown when the external API call fails.
        /// </summary>
        [Test]
        public void GetLatestBlockchainDataAsync_ShouldThrowException_WhenApiCallFails()
        {
            // Arrange
            var currency = "btc.main";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(""),
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act & Assert
            Assert.ThrowsAsync<BlockcypherApiException>(async () =>
                await _blockchainService.GetLatestBlockchainDataAsync(currency, 10));
        }

        /// <summary>
        /// Ensures that an exception is thrown when deserialization of API response fails.
        /// </summary>
        [Test]
        public void GetLatestBlockchainDataAsync_ShouldThrowException_WhenDeserializationFails()
        {
            // Arrange
            var currency = "btc.main";

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("INVALID_JSON")
                });

            // Act & Assert
            Assert.ThrowsAsync<BlockcypherDataDeserializationException>(async () =>
                await _blockchainService.GetLatestBlockchainDataAsync(currency, 10));
        }

        /// <summary>
        /// Ensures that an exception is thrown when saving to the database fails.
        /// </summary>
        [Test]
        public void GetLatestBlockchainDataAsync_ShouldThrowException_WhenDatabaseSaveFails()
        {
            // Arrange
            var currency = "btc.main";
            var sampleApiResponse = new BlockchainData(
                "btc.main", 123456, "testHash", "2025-02-02T08:50:05Z", "latestUrl",
                "prevHash", "prevUrl", 10, 5, 1000, 500, 200, 900, "forkHash");

            _mockRepository.Setup(repo => repo.FindByHashOrHeightAsync(sampleApiResponse.Hash, sampleApiResponse.Height))
                .ReturnsAsync((BlockchainData)null);

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<BlockchainData>()))
                .ThrowsAsync(new DatabaseOperationException("Database error", new Exception("Database error, inner")));

            // Act & Assert
            Assert.ThrowsAsync<DatabaseOperationException>(async () =>
                await _blockchainService.GetLatestBlockchainDataAsync(currency, 10));
        }        
    }
}
