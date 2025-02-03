using ICMarkets.BlockchainDataAggregator.Domain;

namespace ICMarkets.BlockchainDataAggregator.Application.DTOs
{
    public class BlockchainDataDto
    {
        public string Name { get; }
        public int Height { get; }
        public string Hash { get; }
        public string Time { get; }
        public string LatestUrl { get; }
        public string PreviousHash { get; }
        public string PreviousUrl { get; }
        public int PeerCount { get; }
        public int UnconfirmedCount { get; }
        public int HighFeePerKb { get; }
        public int MediumFeePerKb { get; }
        public int LowFeePerKb { get; }
        public int LastForkHeight { get; }
        public string LastForkHash { get; }
        public DateTime CreatedAt { get; }

        public BlockchainDataDto(
            string name, int height, string hash, string time, string latestUrl,
            string previousHash, string previousUrl, int peerCount, int unconfirmedCount,
            int highFeePerKb, int mediumFeePerKb, int lowFeePerKb,
            int lastForkHeight, string lastForkHash, DateTime createdAt)
        {
            Name = name;
            Height = height;
            Hash = hash;
            Time = time;
            LatestUrl = latestUrl;
            PreviousHash = previousHash;
            PreviousUrl = previousUrl;
            PeerCount = peerCount;
            UnconfirmedCount = unconfirmedCount;
            HighFeePerKb = highFeePerKb;
            MediumFeePerKb = mediumFeePerKb;
            LowFeePerKb = lowFeePerKb;
            LastForkHeight = lastForkHeight;
            LastForkHash = lastForkHash;
            CreatedAt = createdAt;
        }

        // Factory method to convert from entity to DTO
        public static BlockchainDataDto FromEntity(BlockchainData entity)
        {
            return new BlockchainDataDto(
                entity.Name, entity.Height, entity.Hash, entity.Time, entity.LatestUrl,
                entity.PreviousHash, entity.PreviousUrl, entity.PeerCount, entity.UnconfirmedCount,
                entity.HighFeePerKb, entity.MediumFeePerKb, entity.LowFeePerKb,
                entity.LastForkHeight, entity.LastForkHash, entity.CreatedAt);
        }
    }
}