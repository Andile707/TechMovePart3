using Xunit;
using System.Net.Http;
using TechMove.Service;

namespace TechMove.Tests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectAmount()
        {
            // Arrange
            var httpClient = new HttpClient();
            var service = new CurrencyService(httpClient);

            decimal usdAmount = 100m;
            decimal exchangeRate = 18.50m;

            // Act
            decimal result = service.ConvertUsdToZar(usdAmount, exchangeRate);

            // Assert
            Assert.Equal(1850m, result);
        }
    }
}