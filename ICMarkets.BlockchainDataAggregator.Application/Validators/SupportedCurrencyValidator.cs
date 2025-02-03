using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.BlockchainDataAggregator.Application.Validators
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class SupportedCurrencyAttribute : ValidationAttribute
    {
        private static string[] _supportedCurrencies;

        public SupportedCurrencyAttribute()
        {
            _supportedCurrencies = []; // Optimized default empty array
        }

        public static void LoadCurrencies(IConfiguration configuration)
        {
            var supportedList = configuration.GetSection("BlockchainSettings:SupportedCurrencies").Get<string[]>();

            _supportedCurrencies = supportedList ?? []; // Avoid null lists

            // Ensure all values are lowercase for faster comparison
            for (int i = 0; i < _supportedCurrencies.Length; i++)
            {
                _supportedCurrencies[i] = _supportedCurrencies[i].ToLowerInvariant();
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string currency)
            {
                if (_supportedCurrencies.Length == 0)
                {
                    IConfiguration configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
                    LoadCurrencies(configuration);
                }
                // Use Array.Exists instead of LINQ for better performance
                if (!Array.Exists(_supportedCurrencies, c => c == currency.ToLowerInvariant()))
                {
                    return new ValidationResult($"Unsupported currency: {currency}. Supported currencies: {string.Join(", ", _supportedCurrencies)}");
                }
            }
            return ValidationResult.Success;
        }
    }
}
