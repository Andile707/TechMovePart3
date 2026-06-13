using glms_frontend_web.Models;
using System.Net.Http.Json;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public class ClientApiService : IClientApiService
    {
        private readonly HttpClient _httpClient;

        public ClientApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<List<ClientModel>?> GetClientsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ClientModel>>("api/clients");
        }

        public async Task<ClientModel?> GetClientByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ClientModel>($"api/clients/{id}");
        }

        public async Task<bool> CreateClientAsync(ClientModel client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            return response.IsSuccessStatusCode;
        }
    }
}