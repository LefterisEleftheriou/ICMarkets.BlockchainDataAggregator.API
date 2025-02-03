using ICMarkets.BlockchainDataAggregator.Application.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Legacy;
using System.ComponentModel.DataAnnotations;

namespace ICMarkets.BlockchainDataAggregator.Domain.UnitTests.Validators
{
    public class SupportedCurrencyAttributeTests
    {
        private readonly SupportedCurrencyAttribute _attribute;

        public SupportedCurrencyAttributeTests()
        {
            _attribute = new SupportedCurrencyAttribute();
        }

        [Test]
        public void IsValid_ShouldReturnTrue_ForSupportedCurrency()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IConfiguration)))
                   .Returns(new ConfigurationBuilder()
                       .AddInMemoryCollection(new Dictionary<string, string?>
                       {
                           { "BlockchainSettings:SupportedCurrencies:0", "btc.main" },
                           { "BlockchainSettings:SupportedCurrencies:1", "btc.test3" },
                           { "BlockchainSettings:SupportedCurrencies:2", "eth.main" },
                           { "BlockchainSettings:SupportedCurrencies:3", "dash.main" },
                           { "BlockchainSettings:SupportedCurrencies:4", "ltc.main" }
                       })
                       .Build());
            var validationContext = new ValidationContext(new object(), serviceProvider.Object, null);

            // Act
            var result = _attribute.GetValidationResult("btc.main", validationContext);

            // Assert
            ClassicAssert.True(result == ValidationResult.Success);
        }

        [Test]
        public void IsValid_ShouldReturnFalse_ForUnsupportedCurrency()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IConfiguration)))
                           .Returns(new ConfigurationBuilder()
                               .AddInMemoryCollection(new Dictionary<string, string?>())
                               .Build());
            var validationContext = new ValidationContext(new object(), serviceProvider.Object, null);

            // Act
            var result = _attribute.GetValidationResult("randomcoin", validationContext);

            // Assert
            ClassicAssert.False(result == ValidationResult.Success);
        }
    }
}
