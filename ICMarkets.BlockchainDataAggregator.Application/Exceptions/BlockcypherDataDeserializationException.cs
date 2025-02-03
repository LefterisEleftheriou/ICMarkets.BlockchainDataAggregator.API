namespace ICMarkets.BlockchainDataAggregator.Application.Exceptions
{
    public class BlockcypherDataDeserializationException() 
        : Exception("Failed to deserialize blockchain data from API response.")
    { }
}