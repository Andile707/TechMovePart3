using glms_frontend_web.Models;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public interface IContractApiService
    {
        Task<List<ContractModel>?> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task<ContractModel?> GetContractByIdAsync(int id);
        Task<bool> CreateContractAsync(ContractCreateViewModel model);
        Task<bool> UpdateContractAsync(int id, ContractCreateViewModel model);
        Task<bool> DeleteContractAsync(int id);
    }
}