using ICMarkets.BlockchainDataAggregator.Domain;
using ICMarkets.BlockchainDataAggregator.Domain.Interfaces;
using ICMarkets.BlockchainDataAggregator.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ICMarkets.BlockchainDataAggregator.Infrastructure.Repositories
{
    public class BlockchainDataRepository : IBlockchainRepository
    {
        private readonly BlockchainDbContext _context;
        private readonly ILogger<BlockchainDataRepository> _logger;

        public BlockchainDataRepository(BlockchainDbContext context, ILogger<BlockchainDataRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(BlockchainData data)
        {
            try
            {
                _logger.LogInformation("Adding blockchain data: {Currency}, Height={Height}, Hash={Hash}",
                data.Name, data.Height, data.Hash);

                await _context.BlockchainData.AddAsync(data);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Blockchain data added successfully: {Currency}, Height={Height}, Hash={Hash}",
                data.Name, data.Height, data.Hash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error while adding blockchain data");
                throw new DatabaseOperationException("Insert Blockchain Data", ex);
            }
        }

        public async Task AddRangeAsync(IEnumerable<BlockchainData> data)
        {
            try
            {
                var blockDataList = JsonConvert.SerializeObject(data);
                _logger.LogInformation("Adding blockchain data: {blockDataList}", blockDataList);

                await _context.BlockchainData.AddRangeAsync(data);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseOperationException("Insert Blockchain Data Range", ex);
            }
        }

        public async Task<BlockchainData?> FindByHashOrHeightAsync(string hash, int height)
        {
            _logger.LogDebug("Checking if blockchain data exists: Height={Height}, Hash={Hash}", height, hash);

            return await _context.BlockchainData
                .FirstOrDefaultAsync(b => b.Hash == hash || b.Height == height);
        }

        public async Task<IEnumerable<BlockchainData>> GetLatestAsync(string currency, int limit = 10)
        {
            _logger.LogInformation("Fetching latest blockchain data for {Currency}", currency);

            string blockchainName;
            switch (currency.ToLowerInvariant())
            {
                case "btc.main": blockchainName = "btc.main"; break;
                case "btc.test3": blockchainName = "btc.test3"; break;
                case "eth.main": blockchainName = "eth.main"; break;
                case "dash.main": blockchainName = "dash.main"; break;
                case "ltc.main": blockchainName = "ltc.main"; break;
                default: throw new InvalidCurrencyException(currency);
            }

            var data = await _context.BlockchainData
                .Where(b => b.Name.ToLower() == blockchainName.ToLower())
                .OrderByDescending(b => b.CreatedAt)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} records for {Currency}", data.Count, currency);
            return data;
        }
    }
}
