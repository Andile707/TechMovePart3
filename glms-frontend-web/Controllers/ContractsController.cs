using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ContractsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var url = $"api/contracts?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&status={status}";
            var contracts = await _httpClient.GetFromJsonAsync<List<ContractModel>>(url);

            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _httpClient.GetFromJsonAsync<ContractModel>($"api/contracts/{id}");

            if (contract == null)
                return NotFound();

            return View(contract);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _httpClient.PostAsJsonAsync("api/contracts", model);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Could not create contract.");
            return View(model);
        }
    }
}