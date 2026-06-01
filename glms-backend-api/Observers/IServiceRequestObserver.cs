using TechMove.Models;

namespace TechMove.Observers
{
    public interface IServiceRequestObserver
    {
        void Update(ServiceRequestModel serviceRequest);
    }
}
