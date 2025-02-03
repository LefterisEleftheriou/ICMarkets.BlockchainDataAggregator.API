using ICMarkets.BlockchainDataAggregator.Application.Exceptions;
using ICMarkets.BlockchainDataAggregator.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.Seeders;

public class BlockchainDbSeeder
{
    public async Task Seed()
    {
        if (await dbContext.Database.CanConnectAsync())
        {
            if (!dbContext.BlockchainData.Any())
            {
                var blockchainData = GetBlockchains();
                await dbContext.BlockchainData.AddRangeAsync(blockchainData);
                await dbContext.SaveChangesAsync();
            }
        }
        else
        {
            throw new SqliteException("Cannot connect to database", 14);
        }
    }

    private IEnumerable<BlockchainData> GetBlockchains()
    {
        return new List<BlockchainData>();
        
        throw new NotImplementedException();
    }

    //private static readonly string[] SupportedCurrencies = { "btc.main", "btc.test3", "eth.main", "dash.main", "ltc.main" };
    private readonly BlockchainDbContext dbContext;

    public BlockchainDbSeeder(BlockchainDbContext _dbContext)
    {
        dbContext = _dbContext;
    }

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlockchainDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BlockchainDbSeeder>>();
        var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var supportedCurrencies = configuration.GetSection("BlockchainSettings:SupportedCurrencies").Get<List<string>>();
        if (supportedCurrencies == null || !supportedCurrencies.Any())
        {
            logger.LogWarning("No supported currencies found in configuration.");
            return;
        }

        try
        {
            for (int i = 0; i < supportedCurrencies.Count; i++)
            {
                string? currency = supportedCurrencies[i];
                // Skip if data already exists
                if (await context.BlockchainData.AnyAsync(b => b.Name.ToLower() == currency))
                {
                    logger.LogInformation($"Skipping {currency.ToUpper()} seeding, data already exists.");
                    continue;
                }

                logger.LogInformation($"Fetching latest {currency.ToUpper()} data from BlockCypher...");
                var blockchainData = await FetchBlockchainDataAsync(httpClient, logger, currency);

                if (blockchainData != null)
                {
                    await context.BlockchainData.AddAsync(blockchainData);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"{currency.ToUpper()} data stored successfully.");
                }
                else
                {
                    logger.LogWarning($"Skipping {currency.ToUpper()} - API returned null data.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding blockchain data.");
        }
    }

    private static async Task<BlockchainData?> FetchBlockchainDataAsync(HttpClient httpClient, ILogger<BlockchainDbSeeder> logger, string currency)
    {
        string url = currency switch
        {
            "btc.main" => "https://api.blockcypher.com/v1/btc/main",
            "btc.test3" => "https://api.blockcypher.com/v1/btc/test3",
            "eth.main" => "https://api.blockcypher.com/v1/eth/main",
            "dash.main" => "https://api.blockcypher.com/v1/dash/main",
            "ltc.main" => "https://api.blockcypher.com/v1/ltc/main",
            _ => throw new InvalidCurrencyException(currency)
        };

        logger.LogInformation($"Fetching data from {url} for currency {currency}");

        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning($"Failed to fetch data for {currency}. Status code: {response.StatusCode}");
            return null; // API request failed, skip this currency
        }

        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            logger.LogWarning($"API returned empty data for {currency}");
            return null;
        }

        var blockchainData = JsonConvert.DeserializeObject<BlockchainData>(json);

        if (blockchainData == null)
        {
            logger.LogWarning($"Failed to deserialize data for {currency}");
            return null;
        }

        logger.LogInformation($"Successfully fetched and deserialized data for {currency}");

        return new BlockchainData(
            blockchainData.Name, blockchainData.Height, blockchainData.Hash, blockchainData.Time,
            blockchainData.LatestUrl, blockchainData.PreviousHash, blockchainData.PreviousUrl, 
            blockchainData.PeerCount, blockchainData.UnconfirmedCount, blockchainData.HighFeePerKb,
            blockchainData.MediumFeePerKb, blockchainData.LowFeePerKb, blockchainData.LastForkHeight,
            blockchainData.LastForkHash);
    }

}
