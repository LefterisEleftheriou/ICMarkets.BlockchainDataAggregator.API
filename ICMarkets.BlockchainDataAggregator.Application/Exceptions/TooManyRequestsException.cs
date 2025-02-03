using System.Net;

namespace ICMarkets.BlockchainDataAggregator.Application.Exceptions
{
    public class TooManyRequestsException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.TooManyRequests;

        public TooManyRequestsException() : base("BlockCypher API rate limit exceeded. Please try again later.") { }
    }
}
