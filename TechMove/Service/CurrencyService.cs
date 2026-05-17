using System.Text.Json;

namespace TechMove.Service
{
    

    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var url = "https://open.er-api.com/v6/latest/USD";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(json);

            var zarRate = doc
                .RootElement
                .GetProperty("rates")
                .GetProperty("ZAR")
                .GetDecimal();

            return zarRate;
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            return usdAmount * exchangeRate;
        }
    }
}
