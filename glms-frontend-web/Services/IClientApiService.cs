using glms_frontend_web.Models;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public interface IClientApiService
    {
        Task<List<ClientModel>?> GetClientsAsync();
        Task<ClientModel?> GetClientByIdAsync(int id);
        Task<bool> CreateClientAsync(ClientModel client);

        Task<bool> UpdateClientAsync(int id, ClientModel client);
        Task<bool> DeleteClientAsync(int id);
    }
}