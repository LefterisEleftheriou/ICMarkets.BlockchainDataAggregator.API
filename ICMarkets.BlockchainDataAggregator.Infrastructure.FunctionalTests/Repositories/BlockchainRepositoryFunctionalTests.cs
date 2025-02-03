using ICMarkets.BlockchainDataAggregator.Application.Exceptions;
using ICMarkets.BlockchainDataAggregator.Domain;
using ICMarkets.BlockchainDataAggregator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.FunctionalTests.Repositories
{
    [TestFixture]
    public class BlockchainRepositoryFunctionalTests
    {
        private BlockchainDbContext _dbContext;
        private BlockchainDataRepository _repository;
        private Mock<ILogger<BlockchainDataRepository>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<BlockchainDataRepository>>();

            var options = new DbContextOptionsBuilder<BlockchainDbContext>()
                .UseInMemoryDatabase(databaseName: "BlockchainFunctionalTestDb")                
                .Options;

            _dbContext = new BlockchainDbContext(options);
            _repository = new BlockchainDataRepository(_dbContext, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted(); // Cleanup DB after each test
            _dbContext.Dispose();
        }

        /// <summary>
        /// Ensures that blockchain data is correctly stored in the database.
        /// </summary>
        [Test]
        public async Task AddAsync_ShouldStoreBlockchainData()
        {
            // Arrange
            var blockchainData = new BlockchainData("btc.main", 123456, "testHash", "2025-02-02T08:50:05Z", "latestUrl",
                "prevHash", "prevUrl", 10, 5, 1000, 500, 200, 900, "forkHash");

            // Act
            await _repository.AddAsync(blockchainData);
            var storedData = await _dbContext.BlockchainData.FirstOrDefaultAsync(b => b.Hash == "testHash");

            // Assert
            Assert.That(storedData, Is.Not.Null);
            Assert.That(storedData.Name, Is.EqualTo("btc.main"));
        }

        /// <summary>
        /// Ensures that retrieving the latest blockchain data returns records in descending order by CreatedAt.
        /// </summary>
        [Test]
        public async Task GetLatestAsync_ShouldReturnDataInDescendingOrder()
        {
            // Arrange
            var data1 = new BlockchainData("btc.main", 100, "hash1", "2025-02-02T08:00:00Z", "latestUrl",
                "prevHash1", "prevUrl1", 10, 5, 1000, 500, 200, 900, "forkHash1");

            var data2 = new BlockchainData("btc.main", 101, "hash2", "2025-02-02T09:00:00Z", "latestUrl",
                "prevHash2", "prevUrl2", 12, 6, 1100, 600, 300, 950, "forkHash2");

            await _repository.AddAsync(data1);
            await _repository.AddAsync(data2);

            // Act
            var latestData = await _repository.GetLatestAsync("btc.main", 2);

            // Assert
            Assert.That(2, Is.EqualTo(latestData.Count()));
            Assert.That(latestData.First().Hash, Is.EqualTo("hash2"));
        }

        /// <summary>
        /// Ensures that duplicate blockchain records (same hash or height) are not inserted.
        /// </summary>
        [Test]
        public void AddAsync_ShouldThrowException_WhenDuplicateHashOrHeight()
        {
            // Arrange
            var data = new BlockchainData("btc.main", 203, "duplicateHash3", "2025-02-02T10:00:00Z", "latestUrl",
                "prevHash", "prevUrl", 15, 7, 1200, 700, 400, 1000, "forkHash");

            // Act
            Assert.DoesNotThrowAsync(() => _repository.AddAsync(data)); // First insert should pass

            var duplicateData = new BlockchainData("btc.main", 203, "duplicateHash3", "2025-02-02T10:00:00Z",
                "latestUrl", "prevHash", "prevUrl", 15, 7, 1200, 700, 400, 1000, "forkHash");

            // Second insert should fail
            Assert.ThrowsAsync<DatabaseOperationException>(() => _repository.AddAsync(duplicateData));
        }

        /// <summary>
        /// Ensures that FindByHashOrHeightAsync returns null when no matching data exists.
        /// </summary>
        [Test]
        public async Task FindByHashOrHeightAsync_ShouldReturnNull_WhenNoMatchingData()
        {
            // Act
            var result = await _repository.FindByHashOrHeightAsync("nonexistentHash", 999999);

            // Assert
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Ensures that an exception is thrown when the database operation fails.
        /// </summary>
        [Test]
        public void AddAsync_ShouldThrowException_WhenDatabaseFails()
        {
            // Arrange
            var invalidData = new BlockchainData(null, 123, "invalidHash", "2025-02-02T11:00:00Z", "latestUrl",
                "prevHash", "prevUrl", 15, 7, 1200, 700, 400, 1000, "forkHash");

            // Act & Assert
            Assert.ThrowsAsync<DatabaseOperationException>(async () => await _repository.AddAsync(invalidData));
        }
    }
}
