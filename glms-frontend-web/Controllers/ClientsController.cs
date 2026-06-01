using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ClientsController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClientsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _httpClient.GetFromJsonAsync<List<ClientModel>>("api/clients");
            return View(clients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _httpClient.GetFromJsonAsync<ClientModel>($"api/clients/{id}");

            if (client == null)
                return NotFound();

            return View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientModel client)
        {
            if (!ModelState.IsValid)
                return View(client);

            var response = await _httpClient.PostAsJsonAsync("api/clients", client);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Could not create client.");
            return View(client);
        }
    }
}