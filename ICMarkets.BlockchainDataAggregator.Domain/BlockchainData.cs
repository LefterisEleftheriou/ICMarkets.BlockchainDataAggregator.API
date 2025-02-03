using Newtonsoft.Json;

namespace ICMarkets.BlockchainDataAggregator.Domain;

public class BlockchainData
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public int Height { get; private set; }
    public string Hash { get; private set; }
    public string Time { get; private set; }

    [JsonProperty("latest_url")]
    public string LatestUrl { get; private set; }

    [JsonProperty("previous_hash")]
    public string PreviousHash { get; private set; }

    [JsonProperty("previous_url")]
    public string PreviousUrl { get; private set; }

    [JsonProperty("peer_count")]
    public int PeerCount { get; private set; }

    [JsonProperty("unconfirmed_count")]
    public int UnconfirmedCount { get; private set; }

    [JsonProperty("high_fee_per_kb")]
    public int HighFeePerKb { get; private set; }

    [JsonProperty("medium_fee_per_kb")]
    public int MediumFeePerKb { get; private set; }

    [JsonProperty("low_fee_per_kb")]
    public int LowFeePerKb { get; private set; }

    [JsonProperty("last_fork_height")]
    public int LastForkHeight { get; private set; }

    [JsonProperty("last_fork_hash")]
    public string LastForkHash { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private BlockchainData() { } // Required for EF Core

    public BlockchainData(string name, int height, string hash, string time, 
        string latestUrl, string previousHash, string previousUrl, int peerCount,
        int unconfirmedCount, int highFeePerKb, int mediumFeePerKb, int lowFeePerKb,
        int lastForkHeight, string lastForkHash)
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
    }
}
