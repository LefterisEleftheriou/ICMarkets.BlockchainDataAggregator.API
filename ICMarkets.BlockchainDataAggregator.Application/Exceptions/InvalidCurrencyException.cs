namespace ICMarkets.BlockchainDataAggregator.Application.Exceptions
{
    public class InvalidCurrencyException(string currency) : Exception($"The currency '{currency}' is not supported.")
    { }
}