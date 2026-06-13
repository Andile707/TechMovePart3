using glms_frontend_web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public class ClientApiService : IClientApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext?
                .Session
                .GetString("JwtToken");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ClientModel>?> GetClientsAsync()
        {
            AddJwtToken();
            return await _httpClient.GetFromJsonAsync<List<ClientModel>>("api/clients");
        }

        public async Task<ClientModel?> GetClientByIdAsync(int id)
        {
            AddJwtToken();
            return await _httpClient.GetFromJsonAsync<ClientModel>($"api/clients/{id}");
        }

        public async Task<bool> CreateClientAsync(ClientModel client)
        {
            AddJwtToken();
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateClientAsync(int id, ClientModel client)
        {
            AddJwtToken();

            var response = await _httpClient.PutAsJsonAsync($"api/clients/{id}", client);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            AddJwtToken();
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}