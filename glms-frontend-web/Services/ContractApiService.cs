using glms_frontend_web.Models;
using System.Net.Http.Json;
using TechMove.Models;

namespace glms_frontend_web.Services
{
    public class ContractApiService : IContractApiService
    {
        private readonly HttpClient _httpClient;

        public ContractApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
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
    }
}