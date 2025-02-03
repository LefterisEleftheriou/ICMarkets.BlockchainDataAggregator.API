using ICMarkets.BlockchainDataAggregator.Application.DTOs;
using ICMarkets.BlockchainDataAggregator.Application.Exceptions;
using ICMarkets.BlockchainDataAggregator.Application.Interfaces;
using ICMarkets.BlockchainDataAggregator.Domain;
using ICMarkets.BlockchainDataAggregator.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ICMarkets.BlockchainDataAggregator.Application.Services
{
    /// <summary>
    /// Service responsible for fetching blockchain data and storing it in the database.
    /// Implements caching and performance optimizations.
    /// </summary>
    public class BlockchainService : IBlockchainService
    {
        private readonly IBlockchainRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BlockchainService> _logger;

        public BlockchainService(IBlockchainRepository repository, HttpClient httpClient, ILogger<BlockchainService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Fetches the latest blockchain data for the given currency.
        /// </summary>
        /// <param name="currency">The cryptocurrency (e.g., "btc.main", "eth.main").</param>
        /// <param name="limit">Number of records to fetch.</param>
        /// <returns>A list of BlockchainDataDto objects.</returns>
        public async Task<IEnumerable<BlockchainDataDto>> GetLatestBlockchainDataAsync(string currency, int limit = 10)
        {
            _logger.LogInformation("Fetching latest blockchain data for {Currency}", currency);

            string url = currency.ToLower() switch
            {
                "eth.main" => "https://api.blockcypher.com/v1/eth/main",
                "dash.main" => "https://api.blockcypher.com/v1/dash/main",
                "btc.main" => "https://api.blockcypher.com/v1/btc/main",
                "btc.test3" => "https://api.blockcypher.com/v1/btc/test3",
                "ltc.main" => "https://api.blockcypher.com/v1/ltc/main",
                _ => throw new InvalidCurrencyException(currency)
            };

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling {Url}", url);
                throw new BlockcypherApiException($"Failed to fetch data from {url}");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Received non-success status code {StatusCode} from {Url}", response.StatusCode, url);
                throw new BlockcypherApiException($"Failed to fetch data from {url}. Status Code: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();
            var blockchainData = JsonConvert.DeserializeObject<BlockchainData>(json);

            if (blockchainData == null)
            {
                _logger.LogError("Failed to deserialize blockchain data for {Currency}", currency);
                throw new BlockcypherDataDeserializationException();
            }

            _logger.LogInformation("Fetched blockchain data for {Currency}: Height={Height}, Hash={Hash}",
            currency, blockchainData.Height, blockchainData.Hash);

            // Check if data already exists (based on Hash or Height)
            var existingData = await _repository.FindByHashOrHeightAsync(blockchainData.Hash, blockchainData.Height);
            if (existingData == null)
            {
                blockchainData = new BlockchainData(
                    blockchainData.Name, blockchainData.Height, blockchainData.Hash, blockchainData.Time, blockchainData.LatestUrl,
                    blockchainData.PreviousHash, blockchainData.PreviousUrl, blockchainData.PeerCount, blockchainData.UnconfirmedCount,
                    blockchainData.HighFeePerKb, blockchainData.MediumFeePerKb, blockchainData.LowFeePerKb,
                    blockchainData.LastForkHeight, blockchainData.LastForkHash
                );

                try
                {
                    await _repository.AddAsync(blockchainData);

                    _logger.LogInformation("New blockchain data added for {Currency}: Height={Height}, Hash={Hash}",
                    currency, blockchainData.Height, blockchainData.Hash);
                }
                catch (Exception ex)
                {
                    throw new DatabaseOperationException("Insert Blockchain Data", ex);
                }
            }

            // Return stored data from DB
            var storedData = await _repository.GetLatestAsync(currency, limit);
            return storedData.Select(BlockchainDataDto.FromEntity);
        }
    }
}
