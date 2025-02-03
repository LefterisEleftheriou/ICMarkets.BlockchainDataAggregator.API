namespace ICMarkets.BlockchainDataAggregator.Domain.Interfaces
{
    public interface IBlockchainRepository
    {
        Task AddAsync(BlockchainData data);
        Task AddRangeAsync(IEnumerable<BlockchainData> data);
        Task<BlockchainData?> FindByHashOrHeightAsync(string hash, int height);
        Task<IEnumerable<BlockchainData>> GetLatestAsync(string currency, int limit = 10);
    }
}
