namespace ICMarkets.BlockchainDataAggregator.Application.Exceptions
{
    public class BlockcypherApiException(string message) 
        : Exception($"Blockcypher API error: {message}")
    { }
}