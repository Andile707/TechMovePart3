using glms_frontend_web.Models;
using System.Net.Http.Json;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public class ContractApiService : IContractApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContractApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
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

        public async Task<List<ContractModel>?> GetContractsAsync(
            DateTime? startDate,
            DateTime? endDate,
            ContractStatus? status)
        {
            var url = $"api/contracts?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&status={status}";
            return await _httpClient.GetFromJsonAsync<List<ContractModel>>(url);
        }

        public async Task<ContractModel?> GetContractByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ContractModel>($"api/contracts/{id}");
        }

        public async Task<bool> CreateContractAsync(ContractCreateViewModel model)
        {
            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.ClientId.ToString()), "ClientId");
            formData.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
            formData.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
            formData.Add(new StringContent(model.Status.ToString()), "Status");
            formData.Add(
                 new StringContent(model.ServiceLevel.ToString()),
                     "ServiceLevel"
             );

            if (model.SignedAgreement != null && model.SignedAgreement.Length > 0)
            {
                var fileContent = new StreamContent(model.SignedAgreement.OpenReadStream());
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(model.SignedAgreement.ContentType);

                formData.Add(fileContent, "SignedAgreement", model.SignedAgreement.FileName);
            }

            var response = await _httpClient.PostAsync("api/contracts", formData);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateContractAsync(int id, ContractCreateViewModel model)
        {
            AddJwtToken();

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.ClientId.ToString()), "ClientId");
            formData.Add(new StringContent(model.StartDate.ToString("yyyy-MM-dd")), "StartDate");
            formData.Add(new StringContent(model.EndDate.ToString("yyyy-MM-dd")), "EndDate");
            formData.Add(new StringContent(model.Status.ToString()), "Status");
            formData.Add(new StringContent(model.ServiceLevel.ToString()), "ServiceLevel");

            if (model.SignedAgreement != null && model.SignedAgreement.Length > 0)
            {
                var fileContent = new StreamContent(model.SignedAgreement.OpenReadStream());

                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(
                        model.SignedAgreement.ContentType);

                formData.Add(
                    fileContent,
                    "SignedAgreement",
                    model.SignedAgreement.FileName);
            }

            var response = await _httpClient.PutAsync($"api/contracts/{id}", formData);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}