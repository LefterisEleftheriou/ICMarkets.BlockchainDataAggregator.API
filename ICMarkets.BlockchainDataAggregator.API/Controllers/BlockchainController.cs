using ICMarkets.BlockchainDataAggregator.Application.DTOs;
using ICMarkets.BlockchainDataAggregator.Application.Interfaces;
using ICMarkets.BlockchainDataAggregator.Application.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ICMarkets.BlockchainDataAggregator.API.Controllers
{
    /// <summary>
    /// API Controller for fetching blockchain data.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainController : ControllerBase
    {
        private readonly IBlockchainService _service;
        private readonly ILogger<BlockchainController> _logger;

        public BlockchainController(IBlockchainService service, ILogger<BlockchainController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the latest blockchain data for a given cryptocurrency.
        /// </summary>
        /// <param name="currency">Currency (must be in the supported list).</param>
        /// <param name="limit">The number of records to return.</param>
        /// <returns>A list of blockchain data.</returns>
        [HttpGet("{currency}")]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
        [ProducesResponseType(typeof(IEnumerable<BlockchainDataDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLatestBlockchainData([FromRoute, SupportedCurrency] string currency, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("API request received for latest blockchain data: {Currency}", currency);

            var data = await _service.GetLatestBlockchainDataAsync(currency, limit);

            if (data is null || data.Count() == 0  )
            {
                _logger.LogWarning("No blockchain data found for {Currency}", currency);
                return NotFound(new { message = $"No data found for {currency}" });
            }

            return Ok(data);
        }
    }
}
