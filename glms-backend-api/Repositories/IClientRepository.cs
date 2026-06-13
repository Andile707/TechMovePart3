using TechMove.Models;

namespace TechMove.Repositories
{
    public interface IClientRepository
    {
        Task<IEnumerable<ClientModel>> GetAllAsync();
        Task<ClientModel?> GetByIdAsync(int id);
        Task<ClientModel> CreateAsync(ClientModel client);
        Task<bool> UpdateAsync(int id, ClientModel client);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}