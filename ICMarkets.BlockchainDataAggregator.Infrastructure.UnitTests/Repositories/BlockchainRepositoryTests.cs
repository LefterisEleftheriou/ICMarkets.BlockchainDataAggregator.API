using ICMarkets.BlockchainDataAggregator.Domain;
using ICMarkets.BlockchainDataAggregator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.UnitTests.Repositories
{
    [TestFixture]
    public class BlockchainRepositoryTests
    {
        private BlockchainDbContext _dbContext;
        private BlockchainDataRepository _repository;
        private Mock<ILogger<BlockchainDataRepository>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<BlockchainDataRepository>>();

            var options = new DbContextOptionsBuilder<BlockchainDbContext>()
                .UseInMemoryDatabase(databaseName: "BlockchainTestDb")
                .Options;

            _dbContext = new BlockchainDbContext(options);
            _repository = new BlockchainDataRepository(_dbContext, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldStoreBlockchainData()
        {
            // Arrange
            var blockchainData = new BlockchainData("btc.main", 123456, "testHash", "2025-02-02T08:50:05Z", "latestUrl",
                "prevHash", "prevUrl", 10, 5, 1000, 500, 200, 900, "forkHash");

            // Act
            await _repository.AddAsync(blockchainData);
            var storedData = await _repository.GetLatestAsync("btc.main", 1);

            // Assert
            Assert.That(storedData, Is.Not.Null);
            Assert.That(storedData.Count(), Is.EqualTo(1));
            Assert.That(storedData.First().Name, Is.EqualTo("btc.main"));
        }
    }
}
