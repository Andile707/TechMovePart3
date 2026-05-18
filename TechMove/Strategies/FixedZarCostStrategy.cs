using TechMove.Models;

namespace TechMove.Strategies
{
    public class FixedZarCostStrategy : IServiceCostStrategy
    {
        public Task<decimal> CalculateCostZarAsync(ServiceRequestModel serviceRequest)
        {
            return Task.FromResult(serviceRequest.CostZAR);
        }
    }
}