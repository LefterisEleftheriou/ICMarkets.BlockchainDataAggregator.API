using ICMarkets.BlockchainDataAggregator.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace ICMarkets.BlockchainDataAggregator.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new { message = exception.Message };
            var json = JsonConvert.SerializeObject(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                InvalidCurrencyException => (int)HttpStatusCode.BadRequest,
                BlockcypherApiException => (int)HttpStatusCode.ServiceUnavailable,
                BlockcypherDataDeserializationException => (int)HttpStatusCode.InternalServerError,
                DatabaseOperationException => (int)HttpStatusCode.InternalServerError,
                TooManyRequestsException => (int)HttpStatusCode.TooManyRequests,
                _ => (int)HttpStatusCode.InternalServerError
            };

            return context.Response.WriteAsync(json);
        }
    }
}
