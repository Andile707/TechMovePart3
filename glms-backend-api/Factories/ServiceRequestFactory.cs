using TechMove.Enums;
using TechMove.Models;

namespace TechMove.Factories
{
    public class ServiceRequestFactory : IServiceRequestFactory
    {
        public ServiceRequestModel Create(
            int contractId,
            string description,
            decimal costUsd,
            decimal costZar,
            ServiceRequestStatus status
        )
        {
            return new ServiceRequestModel
            {
                ContractId = contractId,
                Description = description,
                CostUSD = costUsd,
                CostZAR = costZar,
                Status = status
            };
        }
    }
}
