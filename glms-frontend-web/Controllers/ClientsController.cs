using glms_frontend_web.Models;
using glms_frontend_web.Services;
using Microsoft.AspNetCore.Mvc;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientApiService _clientApiService;

        public ClientsController(IClientApiService clientApiService)
        {
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientApiService.GetClientsAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientApiService.GetClientByIdAsync(id);

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

            var success = await _clientApiService.CreateClientAsync(client);

            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Could not create client.");
            return View(client);
        }
    }
}