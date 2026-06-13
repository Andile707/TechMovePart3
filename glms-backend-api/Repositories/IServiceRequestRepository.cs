using TechMove.Models;

namespace glms_backend_api.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequestModel>> GetAllAsync();
        Task<ServiceRequestModel?> GetByIdAsync(int id);
        Task<ServiceRequestModel?> GetPlainByIdAsync(int id);
        Task<ServiceRequestModel> CreateAsync(ServiceRequestModel serviceRequest);
        Task<ServiceRequestModel> UpdateAsync(ServiceRequestModel serviceRequest);
        Task DeleteAsync(ServiceRequestModel serviceRequest);
        Task<bool> ExistsAsync(int id);
    }
}