using TechMove.Enums;
using TechMove.Models;

namespace glms_backend_api.Repositories
{
    public interface IContractRepository
    {
        Task<IEnumerable<ContractModel>> GetAllAsync(
            DateTime? startDate,
            DateTime? endDate,
            ContractStatus? status);

        Task<ContractModel?> GetByIdAsync(int id);

        Task<ContractModel?> GetPlainByIdAsync(int id);

        Task<ContractModel> CreateAsync(ContractModel contract);

        Task<ContractModel> UpdateAsync(ContractModel contract);

        Task DeleteAsync(ContractModel contract);
    }
}