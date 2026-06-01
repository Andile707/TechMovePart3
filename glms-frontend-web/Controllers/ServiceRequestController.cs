using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests =
                await _httpClient.GetFromJsonAsync<List<ServiceRequestModel>>("api/servicerequests");

            return View(serviceRequests);
        }

        public async Task<IActionResult> Create()
        {
            var contracts =
                await _httpClient.GetFromJsonAsync<List<ContractModel>>("api/contracts");

            ViewBag.ContractId = new SelectList(
                contracts,
                "ContractId",
                "ServiceLevel"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestModel serviceRequest)
        {
            if (!ModelState.IsValid)
                return View(serviceRequest);

            var response =
                await _httpClient.PostAsJsonAsync("api/servicerequests", serviceRequest);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Could not create service request.");
            return View(serviceRequest);
        }

        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest =
                await _httpClient.GetFromJsonAsync<ServiceRequestModel>($"api/servicerequests/{id}");

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }
    }
}