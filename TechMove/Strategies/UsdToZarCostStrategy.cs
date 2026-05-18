using TechMove.Models;
using TechMove.Service;

namespace TechMove.Strategies
{
    public class UsdToZarCostStrategy : IServiceCostStrategy
    {
        private readonly CurrencyService _currencyService;

        public UsdToZarCostStrategy(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public async Task<decimal> CalculateCostZarAsync(ServiceRequestModel serviceRequest)
        {
            decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();

            return _currencyService.ConvertUsdToZar(
                serviceRequest.CostUSD,
                exchangeRate
            );
        }
    }
}