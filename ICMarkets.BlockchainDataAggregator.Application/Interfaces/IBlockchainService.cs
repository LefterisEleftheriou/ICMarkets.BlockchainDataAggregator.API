using ICMarkets.BlockchainDataAggregator.Application.DTOs;

namespace ICMarkets.BlockchainDataAggregator.Application.Interfaces
{
    public interface IBlockchainService
    {
        Task<IEnumerable<BlockchainDataDto>> GetLatestBlockchainDataAsync(string currency, int limit = 10);
    }
}
