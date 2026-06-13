using glms_frontend_web.Models;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public interface IServiceRequestApiService
    {
        Task<List<ServiceRequestModel>?> GetServiceRequestsAsync();
        Task<ServiceRequestModel?> GetServiceRequestByIdAsync(int id);
        Task<bool> CreateServiceRequestAsync(ServiceRequestModel serviceRequest);
    }
}