using TechMove.Models;

namespace TechMove.Strategies
{
    public interface IServiceCostStrategy
    {
        Task<decimal> CalculateCostZarAsync(ServiceRequestModel serviceRequest);
    }
}