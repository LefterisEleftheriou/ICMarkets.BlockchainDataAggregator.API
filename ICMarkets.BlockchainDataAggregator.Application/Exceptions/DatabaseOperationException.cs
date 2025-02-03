namespace ICMarkets.BlockchainDataAggregator.Application.Exceptions
{
    public class DatabaseOperationException(string operation, Exception innerException) 
        : Exception($"Database operation '{operation}' failed.", innerException)
    { }
}