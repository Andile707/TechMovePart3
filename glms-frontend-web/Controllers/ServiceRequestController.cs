using glms_frontend_web.Models;
using glms_frontend_web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMove.Models;

namespace TechMove.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestApiService _serviceRequestApiService;
        private readonly IContractApiService _contractApiService;

        public ServiceRequestController(
            IServiceRequestApiService serviceRequestApiService,
            IContractApiService contractApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
            _contractApiService = contractApiService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestApiService.GetServiceRequestsAsync();
            return View(serviceRequests);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateContractsDropdown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestModel serviceRequest)
        {
            if (!ModelState.IsValid)
            {
                await PopulateContractsDropdown();
                return View(serviceRequest);
            }

            var success = await _serviceRequestApiService.CreateServiceRequestAsync(serviceRequest);

            if (success)
                return RedirectToAction(nameof(Index));

            await PopulateContractsDropdown();
            ModelState.AddModelError("", "Could not create service request.");
            return View(serviceRequest);
        }

        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest = await _serviceRequestApiService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
                return NotFound();

            return View(serviceRequest);
        }

        private async Task PopulateContractsDropdown()
        {
            var contracts = await _contractApiService.GetContractsAsync(null, null, null);

            ViewBag.ContractId = new SelectList(
                contracts,
                "ContractId",
                "ServiceLevel"
            );
        }
    }
}