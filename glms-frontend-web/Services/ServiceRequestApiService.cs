using glms_frontend_web.Models;
using System.Net.Http.Json;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public class ServiceRequestApiService : IServiceRequestApiService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
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
    }
}