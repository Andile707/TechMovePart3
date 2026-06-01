using TechMove.Enums;
using TechMove.Models;

namespace TechMove.Factories
{
    public interface IServiceRequestFactory
    {
        ServiceRequestModel Create(
            int contractId,
            string description,
            decimal costUsd,
            decimal costZar,
            ServiceRequestStatus status
        );
    }
}
