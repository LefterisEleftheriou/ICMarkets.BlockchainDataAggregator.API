using ICMarkets.BlockchainDataAggregator.Application.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;

namespace ICMarkets.BlockchainDataAggregator.Domain.UnitTests.Validators
{
    public class SupportedCurrencyAttributeTests
    {
        private readonly SupportedCurrencyAttribute _attribute;

        public SupportedCurrencyAttributeTests()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "BlockchainSettings:SupportedCurrencies:0", "btc.main" },
                    { "BlockchainSettings:SupportedCurrencies:1", "btc.test3" },
                    { "BlockchainSettings:SupportedCurrencies:2", "eth.main" },
                    { "BlockchainSettings:SupportedCurrencies:3", "dash.main" },
                    { "BlockchainSettings:SupportedCurrencies:4", "ltc.main" }
                })
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddSingleton<ILogger<SupportedCurrencyAttribute>>(new Mock<ILogger<SupportedCurrencyAttribute>>().Object)
                .BuildServiceProvider();

            SupportedCurrencyAttribute.LoadCurrencies(serviceProvider);
            _attribute = new SupportedCurrencyAttribute();
        }

        [Test]
        public void IsValid_ShouldReturnTrue_ForSupportedCurrency()
        {
            // Act
            var result = _attribute.IsValid("btc.main");

            // Assert
            ClassicAssert.True(result);
        }

        [Test]
        public void IsValid_ShouldReturnFalse_ForUnsupportedCurrency()
        {
            // Act
            var result = _attribute.IsValid("randomcoin");

            // Assert
            ClassicAssert.False(result);
        }
    }
}
