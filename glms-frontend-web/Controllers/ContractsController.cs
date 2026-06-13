using Microsoft.AspNetCore.Mvc;
using glms_frontend_web.Services;
using TechMove.Models;
using glms_frontend_web.Models;

namespace TechMove.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractApiService _contractApiService;

        public ContractsController(IContractApiService contractApiService)
        {
            _contractApiService = contractApiService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _contractApiService.GetContractsAsync(startDate, endDate, status);
            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractApiService.GetContractByIdAsync(id);

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

            var success = await _contractApiService.CreateContractAsync(model);

            if (success)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Could not create contract.");
            return View(model);
        }
    }
}