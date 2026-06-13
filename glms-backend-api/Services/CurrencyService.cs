using System.Text.Json;

namespace TechMove.Service
{
    

    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private const string ExchangeRateUrl =
    "https://open.er-api.com/v6/latest/USD";

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ExchangeRateUrl);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);

                return doc.RootElement
                    .GetProperty("rates")
                    .GetProperty("ZAR")
                    .GetDecimal();
            }
            catch
            {
                return 18.50m; // fallback value
            }
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            return usdAmount * exchangeRate;
        }
    }
}
