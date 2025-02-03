using System.Net;
using ICMarkets.BlockchainDataAggregator.Application.DTOs;
using ICMarkets.BlockchainDataAggregator.Domain;
using ICMarkets.BlockchainDataAggregator.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ICMarkets.BlockchainDataAggregator.API.IntegrationTests.Controllers
{
    [TestFixture]
    public class BlockchainControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<BlockchainDbContext>));

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<BlockchainDbContext>(options =>
                        {
                            options.UseSqlite("Filename=ICBlockchainDB.db"); // Real SQLite file for testing
                        });

                        using var scope = services.BuildServiceProvider().CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<BlockchainDbContext>();
                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                    });
                });

            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
            _client.Dispose();
        }

        /// <summary>
        /// Ensures that the API returns 200 OK and valid blockchain data when requested.
        /// </summary>
        [Test]
        public async Task GetLatestBlockchainData_ShouldReturn200OK_WhenDataExists()
        {
            // Arrange - Insert test data
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BlockchainDbContext>();

            var testData = new List<BlockchainData>
            {
                new BlockchainData("btc.main", 100, "hash1", "2025-02-02T08:00:00Z", "latestUrl",
                    "prevHash1", "prevUrl1", 10, 5, 1000, 500, 200, 900, "forkHash1")
            };

            db.BlockchainData.AddRange(testData);
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/blockchain/btc.main");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<BlockchainDataDto>>(content);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.AtLeast(1));
            Assert.That(result[0].Name, Is.EqualTo("btc.main"));
        }

        /// <summary>
        /// Ensures that the API returns 404 Not Found when no blockchain data exists.
        /// </summary>
        [Test]
        public async Task GetLatestBlockchainData_ShouldReturn404_WhenNoDataExists()
        {
            // Act
            var response = await _client.GetAsync("/api/blockchain/btc.main");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        /// <summary>
        /// Ensures that the API returns 400 Bad Request for unsupported currencies.
        /// </summary>
        [Test]
        public async Task GetLatestBlockchainData_ShouldReturn400_WhenCurrencyIsInvalid()
        {
            // Act
            var response = await _client.GetAsync("/api/blockchain/randomcoin");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        /// <summary>
        /// Ensures that the API returns the list of supported currencies.
        /// </summary>
        [Test]
        public async Task GetSupportedCurrencies_ShouldReturn200OK_WithSupportedCurrencies()
        {
            // Act
            var response = await _client.GetAsync("/api/blockchain/supported");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(content);

            Assert.That(result.supportedCurrencies, Is.Not.Null);
        }
    }
}
