using glms_frontend_web.Models;
using System.Net.Http.Json;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceRequestApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
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
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ServiceRequestModel>?> GetServiceRequestsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ServiceRequestModel>>("api/servicerequests");
        }

        public async Task<ServiceRequestModel?> GetServiceRequestByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ServiceRequestModel>($"api/servicerequests/{id}");
        }

        public async Task<bool> CreateServiceRequestAsync(ServiceRequestModel serviceRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/servicerequests", serviceRequest);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateServiceRequestAsync(int id, ServiceRequestModel serviceRequest)
        {
            AddJwtToken();

            var response =
                await _httpClient.PutAsJsonAsync($"api/servicerequests/{id}", serviceRequest);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteServiceRequestAsync(int id)
        {
            var response =
                await _httpClient.DeleteAsync($"api/servicerequests/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}